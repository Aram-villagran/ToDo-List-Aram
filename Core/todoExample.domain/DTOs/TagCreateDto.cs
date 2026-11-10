using System.ComponentModel.DataAnnotations;

namespace todoExample.domain.DTOs
{
    public class TagCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}