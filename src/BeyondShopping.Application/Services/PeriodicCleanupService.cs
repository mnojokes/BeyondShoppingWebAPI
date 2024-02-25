using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeyondShopping.Application.Services;

public class PeriodicCleanupService : BackgroundService
{
    IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _period;
    private readonly int _minutesOldToDelete;


    public PeriodicCleanupService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        double cleanupPeriod;
        if (!double.TryParse(configuration["PeriodicCleanupIntervalMinutes"], out cleanupPeriod))
        {
            throw new ArgumentNullException("PeriodicCleanupIntervalMinutes");
        }
        if (!int.TryParse(configuration["PendingOrderExpiryTimeMinutes"], out _minutesOldToDelete))
        {
            throw new ArgumentNullException("PendingOrderExpiryTimeMinutes");
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
                        OrderService service = scope.ServiceProvider.GetRequiredService<OrderService>();
                        await service.CleanupExpiredOrders(_minutesOldToDelete);
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
