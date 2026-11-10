using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using todoExample.domain.Models.Identity;

namespace todoExample.MysqlDAL.Data.Mappings;

public class ApplicationUserMap
{
    public ApplicationUserMap(EntityTypeBuilder<ApplicationUser> entityBuilder)
    {
        entityBuilder.ToTable("AspNetUsers");
        entityBuilder.HasKey(e => e.Id);
        entityBuilder.Property(e => e.Email).IsRequired().HasMaxLength(256);
       

        entityBuilder.HasMany(e => e.Tags)
            .WithOne(e => e.User) 
            .HasForeignKey(e => e.UserId)
            .HasPrincipalKey(e => e.Id);
    }
}