using System;
using System.Collections.Generic;
using System.Text;

namespace todoExample.domain.Models.Identity
{
    public class TodoTaskTagModel
    {
        public Guid TagId { get; set; } 
        public Guid TodoTaskId { get; set; }       

        public TodoTaskModel? TodoTask { get; set; }
        public TagModel? Tag { get; set; }
    }
}
