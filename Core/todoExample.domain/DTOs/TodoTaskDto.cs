using System;

namespace todoExample.domain.DTOs
{
    public class TodoTaskDto
    {
        public Guid Id { get; set; }
        public Guid TodoListId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
