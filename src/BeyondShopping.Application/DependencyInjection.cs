using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace BeyondShopping.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        services.AddScoped<CreateOrderRequestValidator>();
        services.AddScoped<IdValidator>();
        services.AddHostedService<PeriodicCleanupService>();
    }
}
