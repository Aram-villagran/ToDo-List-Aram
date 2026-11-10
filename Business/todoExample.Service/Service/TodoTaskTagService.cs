using todoExample.domain.DTOs;
using todoExample.domain.Models.Identity;
using todoExample.domain.Respository;
using todoExample.domain.Respository.Base;
using todoExample.domain.Service;

namespace todoExample.Service.Service
{
    public class TodoTaskTagService : ITodoTaskTagService
    {
        private readonly IRepository<TodoTaskModel> _taskRepository;
        private readonly IRepository<TodoListModel> _listRepository;
        private readonly IRepository<TagModel> _tagRepository;
        private readonly ITodoTaskTagRepository _taskTagRepository;

        public TodoTaskTagService(
            IRepository<TodoTaskModel> taskRepository,
            IRepository<TodoListModel> listRepository,
            IRepository<TagModel> tagRepository,
            ITodoTaskTagRepository taskTagRepository)
        {
            _taskRepository = taskRepository;
            _listRepository = listRepository;
            _tagRepository = tagRepository;
            _taskTagRepository = taskTagRepository;
        }

        public async Task<bool> AssignAsync(Guid userId, Guid taskId, Guid tagId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);
            var tag = await GetOwnedTagAsync(userId, tagId);

            if (task == null || tag == null)
            {
                return false;
            }

            if (await _taskTagRepository.ExistsAsync(taskId, tagId))
            {
                throw new InvalidOperationException("La tarea ya tiene asignado ese tag");
            }

            await _taskTagRepository.AddAsync(taskId, tagId);

            return true;
        }

        public async Task<bool> RemoveAsync(Guid userId, Guid taskId, Guid tagId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null || !await _taskTagRepository.ExistsAsync(taskId, tagId))
            {
                return false;
            }

            await _taskTagRepository.RemoveAsync(taskId, tagId);

            return true;
        }

        public async Task<IReadOnlyList<TagDto>?> GetTagsByTaskAsync(Guid userId, Guid taskId)
        {
            var task = await GetOwnedTaskAsync(userId, taskId);

            if (task == null)
            {
                return null;
            }

            var tags = await _taskTagRepository.GetTagsByTaskAsync(taskId);

            return tags.Select(ToTagDto).ToList();
        }

        public async Task<IReadOnlyList<TodoTaskDto>?> GetTasksByTagAsync(Guid userId, Guid tagId)
        {
            var tag = await GetOwnedTagAsync(userId, tagId);

            if (tag == null)
            {
                return null;
            }

            var tasks = await _taskTagRepository.GetTasksByTagAsync(tagId);

            return tasks.Select(ToTaskDto).ToList();
        }

        #region Helpers

        private async Task<TodoTaskModel?> GetOwnedTaskAsync(Guid userId, Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
            {
                return null;
            }

            var list = await _listRepository.GetByIdAsync(task.TodoListId);

            if (list == null || list.UserId != userId)
            {
                return null;
            }

            return task;
        }

        private async Task<TagModel?> GetOwnedTagAsync(Guid userId, Guid tagId)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);

            if (tag == null || tag.UserId != userId)
            {
                return null;
            }

            return tag;
        }

        private static TagDto ToTagDto(TagModel model) => new()
        {
            Id = model.Id,
            UserId = model.UserId,
            Name = model.Name,
            CreatedAt = model.CreatedAt,
            Enabled = model.Enabled
        };

        private static TodoTaskDto ToTaskDto(TodoTaskModel model) => new()
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