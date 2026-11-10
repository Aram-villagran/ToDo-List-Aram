using Microsoft.Extensions.DependencyInjection;
using todoExample.DapperDAL.DbContext;
using todoExample.DapperDAL.Repository;
using todoExample.domain.Respository.Base;

namespace todoExample.DapperDAL;

public static class DependencyContainer
{
    public static IServiceCollection AddDapperDAL(this IServiceCollection services)
    {
        services.AddTransient<IConnectionFactory, ConnectionFactory>();
        services.AddTransient(typeof(IDapperRepository<>), typeof(DapperRepository<>));
        services.AddTransient<IRawDapperRepository, RawDapperRepository>();

        return services;
    }
}