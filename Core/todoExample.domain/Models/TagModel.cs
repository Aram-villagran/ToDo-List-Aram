using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace todoExample.domain.Models.Identity
{
    public class TagModel : BaseEntity
    {        
        public required string Name { get; set; }


        #region Relationship
        public Guid UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual List<TodoTaskTagModel>? TodoTaskTags { get; set; }
        #endregion
    }
}
