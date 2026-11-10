using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using todoExample.domain.Models;
using todoExample.domain.Models.Identity;
using todoExample.MysqlDAL.Data.Mappings;
using todoExample.MysqlDAL.Data.Seeds;

namespace todoExample.MysqlDAL.Data;

public class IdentityMySqlContext: IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly IHttpContextAccessor _context;
    
    public  IdentityMySqlContext(DbContextOptions<IdentityMySqlContext> options, IHttpContextAccessor context) : 
        base(options)
    {
        _context = context;
    }

    public DbSet<TodoTaskModel> TodoTasks { get; set; }
    public DbSet<TodoListModel> TodoLists { get; set; }
    public DbSet<TagModel> Tags { get; set; }
    public DbSet<TodoTaskTagModel> TodoTaskTags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        _ = new ApplicationUserMap(builder.Entity<ApplicationUser>());
        _ = new TodoTaskMap(builder.Entity<TodoTaskModel>());
        _ = new TodoListMap(builder.Entity<TodoListModel>());
        _ = new TagMap(builder.Entity<TagModel>());
        _ = new TaskTagMap(builder.Entity<TodoTaskTagModel>());

        IdentitySeed.Apply(builder);
    }

    #region Extensions

    public override int SaveChanges()
    {
        StampDates();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        StampDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        StampDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    #endregion
    
    #region Helpers

    private void StampDates()
    {
        // UTC so runtime-stamped rows share the same time basis as the UTC seed defaults
        // in BaseEntity.SeedTimestamp — mixing local and UTC would produce inconsistent
        // persisted timestamps and confusing ordering across environments.
        var now = DateTime.UtcNow;

        foreach (var dbEntityEntry in ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Added or EntityState.Modified))
        {
            
            if (dbEntityEntry.Entity is not BaseEntity entity) continue;

            entity.UpdatedAt = now;

            if (dbEntityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
            }
        }
    }

    #endregion
}