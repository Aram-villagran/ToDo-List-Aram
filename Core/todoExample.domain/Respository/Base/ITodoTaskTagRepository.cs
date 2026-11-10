using todoExample.domain.Models.Identity;

namespace todoExample.domain.Respository;

public interface ITodoTaskTagRepository
{
    Task<bool> ExistsAsync(Guid taskId, Guid tagId);
    Task AddAsync(Guid taskId, Guid tagId);
    Task RemoveAsync(Guid taskId, Guid tagId);
    Task<List<TagModel>> GetTagsByTaskAsync(Guid taskId);
    Task<List<TodoTaskModel>> GetTasksByTagAsync(Guid tagId);
}