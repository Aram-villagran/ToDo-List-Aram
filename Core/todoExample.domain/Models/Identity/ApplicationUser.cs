using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace todoExample.domain.Models.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool Enabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    #region Relationship
    public virtual List<TagModel>? Tags { get; set; }
    public virtual List<TodoListModel>? TodoList { get; set; }

    #endregion
}