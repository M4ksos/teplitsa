using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Teplitsa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<GreenhouseDbContext>(o =>
            o.UseSqlite(connectionString, b => b.MigrationsAssembly("Teplitsa.Infrastructure")));
        return services;
    }
}