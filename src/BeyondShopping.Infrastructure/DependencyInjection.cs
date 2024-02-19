using BeyondShopping.Core.Interfaces;
using BeyondShopping.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BeyondShopping.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}
