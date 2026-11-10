using System;
using System.Collections.Generic;
using System.Text;

namespace todoExample.domain.Models.Identity
{
    public class TodoListModel : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }

        #region rekiotship
        public Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual List<TodoTaskModel>? TodoTasks { get; set; } 
        #endregion
    }
}
