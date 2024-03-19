using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeyondShopping.Application.Services;

public class PeriodicCleanupService : BackgroundService
{
    IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _period;


    public PeriodicCleanupService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        double cleanupPeriod;
        if (!double.TryParse(configuration["PeriodicCleanupIntervalMinutes"], out cleanupPeriod))
        {
            throw new ArgumentNullException("PeriodicCleanupIntervalMinutes");
        }

        _period = TimeSpan.FromMinutes(cleanupPeriod);
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (PeriodicTimer timer = new PeriodicTimer(_period))
        {
            while (!stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using (IServiceScope scope = _scopeFactory.CreateScope())
                    {
                        OrderService orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
                        await orderService.CleanupExpiredOrders();
                    }
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Periodic cleanup failed: {ex.Message}");
                }
            }
        }
    }
}
