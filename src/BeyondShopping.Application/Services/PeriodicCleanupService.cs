using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BeyondShopping.Application.Services;

public class PeriodicCleanupService : BackgroundService
{
    private readonly TimeSpan _period;
    private readonly int _minutesOldToDelete;


    public PeriodicCleanupService(IConfiguration configuration)
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
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // TODO: add scope factory
                // TODO: call cleanup from OrderService
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
