using BeyondShopping.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BeyondShopping.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
    }
}
