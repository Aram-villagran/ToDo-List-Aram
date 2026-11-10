using Microsoft.EntityFrameworkCore;
using todoExample.domain.Models.Identity;
using todoExample.domain.Respository;
using todoExample.MysqlDAL.Data;

namespace todoExample.MysqlDAL.Repository;

public class TodoTaskTagRepository : ITodoTaskTagRepository
{
    private readonly IdentityMySqlContext _context;

    public TodoTaskTagRepository(IdentityMySqlContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid taskId, Guid tagId)
    {
        return await _context.Set<TodoTaskTagModel>()
            .AnyAsync(tt => tt.TodoTaskId == taskId && tt.TagId == tagId);
    }

    public async Task AddAsync(Guid taskId, Guid tagId)
    {
        _context.Set<TodoTaskTagModel>().Add(new TodoTaskTagModel
        {
            TodoTaskId = taskId,
            TagId = tagId
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid taskId, Guid tagId)
    {
        var relation = await _context.Set<TodoTaskTagModel>()
            .FirstOrDefaultAsync(tt => tt.TodoTaskId == taskId && tt.TagId == tagId);

        if (relation == null)
        {
            return;
        }

        _context.Set<TodoTaskTagModel>().Remove(relation);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TagModel>> GetTagsByTaskAsync(Guid taskId)
    {
        return await _context.Set<TodoTaskTagModel>()
            .IgnoreQueryFilters()
            .Where(tt => tt.TodoTaskId == taskId)
            .Select(tt => tt.Tag!)
            .OrderByDescending(tg => tg.Enabled)
            .ThenBy(tg => tg.Name)
            .ToListAsync();
    }

    public async Task<List<TodoTaskModel>> GetTasksByTagAsync(Guid tagId)
    {
        return await _context.Set<TodoTaskModel>()
            .Where(t => t.TodoTaskTags!.Any(tt => tt.TagId == tagId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}