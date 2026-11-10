using todoExample.domain.DTOs;

namespace todoExample.domain.Service
{
    public interface ITodoTaskTagService
    {
        Task<bool> AssignAsync(Guid userId, Guid taskId, Guid tagId);
        Task<bool> RemoveAsync(Guid userId, Guid taskId, Guid tagId);
        Task<IReadOnlyList<TagDto>?> GetTagsByTaskAsync(Guid userId, Guid taskId);
        Task<IReadOnlyList<TodoTaskDto>?> GetTasksByTagAsync(Guid userId, Guid tagId);
    }
}
