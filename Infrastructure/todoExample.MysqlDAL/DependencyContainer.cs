using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using todoExample.domain.Models.Identity;
using todoExample.MysqlDAL.Data;

namespace todoExample.MysqlDAL;

public static class DependencyContainer
{
    public static IServiceCollection AddMySqlDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultMySQLConnection");
            
        services.AddDbContext<IdentityMySqlContext>(option =>
            option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        
        // AddIdentityCore no registra Data Protection; los token providers lo necesitan
        services.AddDataProtection();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityMySqlContext>()
            .AddDefaultTokenProviders();
        
        return services;
    }
}