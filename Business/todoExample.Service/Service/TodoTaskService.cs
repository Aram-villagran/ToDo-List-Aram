using Microsoft.AspNetCore.Identity;
using todoExample.domain.DTOs;
using todoExample.domain.Models.Identity;
using todoExample.domain.Respository.Base;
using todoExample.domain.Service;

namespace todoExample.Service.Service
{
    public class TodoTaskService : ITodoTaskService
    {
        private readonly IRepository<TodoTaskModel> _taskRepository;
        private readonly IRepository<TodoListModel> _listRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoTaskService(
            IRepository<TodoTaskModel> taskRepository,
            IRepository<TodoListModel> listRepository,
            UserManager<ApplicationUser> userManager)
        {
            _taskRepository = taskRepository;
            _listRepository = listRepository;
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<TodoTaskDto>?> GetByListAsync(Guid userId, Guid listId)
        {
            var list = await GetOwnedListAsync(userId, listId);

            if (list == null)
            {
                return null;
            }

            var tasks = await _taskRepository.GetAllAsync(
                t => t.TodoListId == listId,
                q => q.OrderByDescending(t => t.CreatedAt));

            return tasks.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<TodoTaskDto>?> GetPendingByListAsync(Guid userId, Guid listId)
        {
            var list = await GetOwnedListAsync(userId, listId);

            if (list == null)
            {
                return null;
            }

            var tasks = await _taskRepository.GetAllAsync(
                t => t.TodoListId == listId && !t.Completed,
                q => q.OrderByDescending(t => t.CreatedAt));

            return tasks.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<TodoTaskDto>> GetPendingByUserAsync(Guid userId)
        {
            var tasks = await _taskRepository.GetAllAsync(
                t => t.TodoList!.UserId == userId && !t.Completed,
                q => q.OrderByDescending(t => t.CreatedAt));

            return tasks.Select(ToDto).ToList();
        }

        public async Task<TodoTaskDto?> GetByIdAsync(Guid userId, Guid taskId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null)
            {
                return null;
            }

            return ToDto(task);
        }

        public async Task<TodoTaskDto?> CreateAsync(Guid userId, Guid listId, TodoTaskCreateDto dto)
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

            var list = await GetOwnedListAsync(userId, listId);

            if (list == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new InvalidOperationException("El titulo de la tarea es obligatorio");
            }

            var model = new TodoTaskModel
            {
                TodoListId = listId,
                Title = dto.Title.Trim(),
                Description = dto.Description,
                DueDate = dto.DueDate,
                Completed = false,
                CompletedAt = null,
                Enabled = true
            };

            await _taskRepository.AddAsync(model);

            return ToDto(model);
        }

        public async Task<TodoTaskDto?> UpdateAsync(Guid userId, Guid taskId, TodoTaskUpdateDto dto)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new InvalidOperationException("El titulo de la tarea es obligatorio");
            }

            task.Title = dto.Title.Trim();
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;

            await _taskRepository.UpdateAsync(task);

            return ToDto(task);
        }

        public async Task<TodoTaskDto?> CompleteAsync(Guid userId, Guid taskId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null)
            {
                return null;
            }

            if (task.Completed)
            {
                throw new InvalidOperationException("La tarea ya esta completada");
            }

            task.Completed = true;
            task.CompletedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return ToDto(task);
        }

        public async Task<TodoTaskDto?> ReopenAsync(Guid userId, Guid taskId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null)
            {
                return null;
            }

            if (!task.Completed)
            {
                throw new InvalidOperationException("Solo una tarea completada puede reabrirse");
            }

            task.Completed = false;
            task.CompletedAt = null;

            await _taskRepository.UpdateAsync(task);

            return ToDto(task);
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid taskId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null)
            {
                return false;
            }

            if (task.Completed)
            {
                throw new InvalidOperationException("Una tarea completada no puede eliminarse");
            }

            await _taskRepository.DeleteAsync(task);

            return true;
        }

        public async Task<IReadOnlyList<TodoTaskDto>> SearchAsync(Guid userId, bool? completed, string? tag, string? text)
        {
            var tagName = tag?.Trim();
            var searchText = text?.Trim();

            var tasks = await _taskRepository.GetAllAsync(
                t => t.TodoList!.UserId == userId
                     && (completed == null || t.Completed == completed)
                     && (string.IsNullOrEmpty(tagName)
                         || t.TodoTaskTags!.Any(tt => tt.Tag!.UserId == userId && tt.Tag.Name == tagName))
                     && (string.IsNullOrEmpty(searchText)
                         || t.Title.Contains(searchText)
                         || (t.Description != null && t.Description.Contains(searchText))),
                q => q.OrderByDescending(t => t.CreatedAt));

            return tasks.Select(ToDto).ToList();
        }

        #region Helpers

        private async Task<TodoListModel?> GetOwnedListAsync(Guid userId, Guid listId)
        {
            var list = await _listRepository.GetByIdAsync(listId);

            if (list == null || list.UserId != userId)
            {
                return null;
            }

            return list;
        }

        private async Task<TodoTaskModel?> GetOwnedTaskAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
            {
                return null;
            }

            var list = await GetOwnedListAsync(userId, task.TodoListId);

            if (list == null)
            {
                return null;
            }

            return task;
        }

        private static TodoTaskDto ToDto(TodoTaskModel model) => new()
        {
            Id = model.Id,
            TodoListId = model.TodoListId,
            Title = model.Title,
            Description = model.Description,
            Completed = model.Completed,
            CompletedAt = model.CompletedAt,
            DueDate = model.DueDate,
            CreatedAt = model.CreatedAt
        };

        #endregion
    }
}