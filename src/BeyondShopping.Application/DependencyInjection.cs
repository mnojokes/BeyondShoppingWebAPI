using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace BeyondShopping.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        services.AddScoped<CreateOrderRequestValidator>();
        services.AddScoped<IdValidator>();
        services.AddHostedService<PeriodicCleanupService>();

        services.AddHttpClient("ClientWithExponentialBackoff")
            .AddPolicyHandler(GetRetryPolicy());
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
