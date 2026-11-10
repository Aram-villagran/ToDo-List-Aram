using System;
using System.Collections.Generic;
using System.Text;
using todoExample.domain.DTOs;

namespace todoExample.domain.Service
{
    public interface ITodoListService
    {
        Task<IEnumerable<TodoListDto>> GetByUserAsync(Guid userId);
        Task<TodoListDto?> GetByIdAsync(Guid userId, Guid listId);
        Task<TodoListDto> CreateAsync(Guid userId, TodoListCreateDto dto);
        Task<TodoListDto?> UpdateAsync(Guid userId, Guid listId, TodoListUpdateDto dto);
        Task<bool> DeleteAsync(Guid userId, Guid listId);
    }
}
