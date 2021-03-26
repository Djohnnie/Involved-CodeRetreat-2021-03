using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InvolvedExchangeWorkers
{
    public class ExchangeWorker : BackgroundService
    {
        private readonly string _signalrUrl = "https://involvedexchangewebapi20210324153741.azurewebsites.net/InvolvedExchange";
        private readonly HubConnection _hubConnection;
        private readonly IExchangeTrend _exchangeTrend;
        private readonly ILogger<ExchangeWorker> _logger;

        public ExchangeWorker(
            IExchangeTrend exchangeTrend,
            ILogger<ExchangeWorker> logger)
        {
            try
            {
                _exchangeTrend = exchangeTrend;
                _logger = logger;

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_signalrUrl)
                    .Build();

                _hubConnection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _hubConnection.StartAsync();
                };

                _hubConnection.On<Guid, String, Double>("CurrencyUpdate", (id, name, value) =>
                {
                    try
                    {
                        _exchangeTrend.AddCurrencyDetail(new CurrencyDetail
                        {
                            Id = id,
                            Name = name,
                            Value = (Decimal)value
                        });
                        _logger.LogInformation($"CurrencyUpdate: {name} [{value:F2}]");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private bool _started = false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_started)
                    {
                        await _hubConnection.StartAsync();
                        _started = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                await Task.Delay(60000);
            }
        }
    }
}