using Microsoft.AspNetCore.Identity;
using todoExample.domain.DTOs;
using todoExample.domain.Models.Identity;
using todoExample.domain.Respository.Base;
using todoExample.domain.Service;

namespace todoExample.Service.Service
{
    public class TodoListService : ITodoListService
    {
        private readonly IRepository<TodoListModel> _listRepository;
        private readonly IRepository<TodoTaskModel> _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoListService(
            IRepository<TodoListModel> listRepository,
            IRepository<TodoTaskModel> taskRepository,
            UserManager<ApplicationUser> userManager)
        {
            _listRepository = listRepository;
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        public async Task<TodoListDto> CreateAsync(Guid userId, TodoListCreateDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new InvalidOperationException("El usuario no existe.");
            }

            if (!user.Enabled)
            {
                throw new InvalidOperationException("El usuario esta deshabilitado");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException("El nombre de la lista es obligatorio");
            }

            var name = dto.Name.Trim();

            var exists = await _listRepository.AnyAsync(l => l.UserId == userId && l.Name == name);

            if (exists)
            {
                throw new InvalidOperationException("Ya existe una lista activa con ese nombre para el usuario.");
            }

            var model = new TodoListModel
            {
                UserId = userId,
                Name = name,
                Description = dto.Description,
                Enabled = true
            };

            await _listRepository.AddAsync(model);

            return ToDto(model);
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid listId)
        {
            var list = await _listRepository.GetByIdAsync(listId);

            if (list == null || list.UserId != userId)
            {
                return false;
            }

            var hasPendingTasks = await _taskRepository.AnyAsync(t => t.TodoListId == listId && !t.Completed);

            if (hasPendingTasks)
            {
                throw new InvalidOperationException("No se puede eliminar una lista con tareas pendientes");
            }

            await _listRepository.DeleteAsync(list);

            return true;
        }

        public async Task<TodoListDto?> GetByIdAsync(Guid userId, Guid listId)
        {
            var list = await _listRepository.GetByIdAsync(listId);

            if (list == null || list.UserId != userId)
            {
                return null;
            }

            return ToDto(list);
        }

        public async Task<IEnumerable<TodoListDto>> GetByUserAsync(Guid userId)
        {
            var lists = await _listRepository.GetAllAsync(
                l => l.UserId == userId,
                q => q.OrderByDescending(l => l.CreatedAt));

            return lists.Select(ToDto);
        }

        public async Task<TodoListDto?> UpdateAsync(Guid userId, Guid listId, TodoListUpdateDto dto)
        {
            var list = await _listRepository.GetByIdAsync(listId);

            if (list == null || list.UserId != userId)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException("El nombre de la lista es obligatorio.");
            }

            var name = dto.Name.Trim();

            var exists = await _listRepository.AnyAsync(l => l.UserId == userId && l.Name == name && l.Id != listId);

            if (exists)
            {
                throw new InvalidOperationException("Ya existe una lista activa con ese nombre para el usuario.");
            }

            list.Name = name;
            list.Description = dto.Description;

            await _listRepository.UpdateAsync(list);

            return ToDto(list);
        }

        private static TodoListDto ToDto(TodoListModel model) => new()
        {
            Id = model.Id,
            UserId = model.UserId,
            Name = model.Name,
            Description = model.Description,
            CreatedAt = model.CreatedAt
        };
    }
}