using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using todoExample.domain.Service;
using todoExample.MysqlDAL;
using todoExample.DapperDAL;
using todoExample.Service.Security;
using todoExample.Service.Service;

namespace todoExample.Service;

public static class DependencyContainer
{
    public static IServiceCollection AddServices(
        this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMySqlDataAccess(configuration);
        services.AddDapperDAL();

        services.AddJwtSettings(configuration);
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITodoListService, TodoListService>();
        services.AddScoped<ITodoTaskService, TodoTaskService>();

        return services;
    }
}
