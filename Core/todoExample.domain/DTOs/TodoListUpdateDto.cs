using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace todoExample.domain.DTOs
{
    public class TodoListUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
       
    }
}
