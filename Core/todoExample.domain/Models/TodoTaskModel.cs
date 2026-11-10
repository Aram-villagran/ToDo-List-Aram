using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace todoExample.domain.Models.Identity
{
    public class TodoTaskModel : BaseEntity
    {
        public required string Title { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }


        #region relationship
    
        public Guid TodoListId { get; set; }
        public virtual TodoListModel? TodoList { get; set; }
        public virtual List<TodoTaskTagModel>? TodoTaskTags{ get; set; }
        #endregion
    }
}