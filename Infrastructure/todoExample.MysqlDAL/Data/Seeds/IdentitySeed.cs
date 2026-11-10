using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using todoExample.domain.Models.Identity;

namespace todoExample.MysqlDAL.Data.Seeds;

/// <summary>
/// Datos iniciales de Identity. Todos los valores son constantes para que la migración sea estable.
/// </summary>
public static class IdentitySeed
{
    public static readonly Guid AdminRoleId = new("6f1c2a8e-3b4d-4e5f-9a10-1b2c3d4e5f60");
    public static readonly Guid UserRoleId = new("7a2d3b9f-4c5e-4f60-8b21-2c3d4e5f6071");
    public static readonly Guid AdminUserId = new("8b3e4c0a-5d6f-4071-9c32-3d4e5f607182");

    private const string AdminEmail = "doe@sacai.com.mx";

    // Hash generado con PasswordHasher<TUser> (Identity v3); la contraseña en claro no se guarda en el código
    //Pass Wk6%izNbTy
    private const string AdminPasswordHash =
        "AQAAAAIAAYagAAAAEL6h2a9g8GAKrKKUSP0GpsIaFDs6zDx59NXgkjuGZOMIF6VGWchLm2OmNEFhFT1Tng==";

    private static readonly DateTime SeedTimestamp = new(2026, 10, 8, 0, 0, 0, DateTimeKind.Utc);

    public static void Apply(ModelBuilder builder)
    {
        builder.Entity<ApplicationRole>().HasData(
            new ApplicationRole
            {
                Id = AdminRoleId,
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "b1f0c7d2-6e5a-4b3c-8d9e-0f1a2b3c4d5e"
            },
            new ApplicationRole
            {
                Id = UserRoleId,
                Name = "user",
                NormalizedName = "USER",
                ConcurrencyStamp = "c2a1d8e3-7f6b-4c4d-9e0f-1a2b3c4d5e6f"
            });

        builder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = AdminUserId,
            UserName = AdminEmail,
            NormalizedUserName = AdminEmail.ToUpperInvariant(),
            Email = AdminEmail,
            NormalizedEmail = AdminEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            PasswordHash = AdminPasswordHash,
            SecurityStamp = "D3B2E9F4A8C74D5E9F0A1B2C3D4E5F60",
            ConcurrencyStamp = "e4c3fa05-9b8d-4e6f-a01b-2c3d4e5f6071",
            LockoutEnabled = true,
            FirstName = "John",
            LastName = "Doe",
            Enabled = true,
            CreatedAt = SeedTimestamp,
            UpdatedAt = SeedTimestamp
        });

        builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
        {
            UserId = AdminUserId,
            RoleId = AdminRoleId
        });
    }
}
