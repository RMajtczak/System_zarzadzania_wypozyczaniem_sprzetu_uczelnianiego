namespace Wypożyczlania_sprzętu.Services;

public class ReservationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReservationBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                reservationService.CloseExpiredReservations();
            }

            // Odczekaj np. 5 minut
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}