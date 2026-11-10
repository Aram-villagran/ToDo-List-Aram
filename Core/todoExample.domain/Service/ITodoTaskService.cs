using System;
using System.Collections.Generic;
using todoExample.domain.DTOs;

namespace todoExample.domain.Service
{
    public interface ITodoTaskService
    {
        Task<IReadOnlyList<TodoTaskDto>?> GetByListAsync(Guid userId, Guid listId);
        Task<IReadOnlyList<TodoTaskDto>?> GetPendingByListAsync(Guid userId, Guid listId);
        Task<IReadOnlyList<TodoTaskDto>> GetPendingByUserAsync(Guid userId);
        Task<TodoTaskDto?> GetByIdAsync(Guid userId, Guid taskId);
        Task<TodoTaskDto?> CreateAsync(Guid userId, Guid listId, TodoTaskCreateDto dto);
        Task<TodoTaskDto?> UpdateAsync(Guid userId, Guid taskId, TodoTaskUpdateDto dto);
        Task<TodoTaskDto?> CompleteAsync(Guid userId, Guid taskId);
        Task<TodoTaskDto?> ReopenAsync(Guid userId, Guid taskId);
        Task<bool> DeleteAsync(Guid userId, Guid taskId);
        Task<IReadOnlyList<TodoTaskDto>> SearchAsync(Guid userId, bool? completed, string? tag, string? text);
    }
}