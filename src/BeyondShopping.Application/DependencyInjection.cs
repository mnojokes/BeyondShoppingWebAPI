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

        services.AddScoped<IdValidator>();
        services.AddScoped<CreateOrderRequestValidator>();

        services.AddHttpClient("ClientWithExponentialBackoff")
            .AddPolicyHandler(GetRetryPolicy());

        services.AddHostedService<PeriodicCleanupService>();
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
