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
        private readonly ILogger<ExchangeWorker> _logger;

        public ExchangeWorker(ILogger<ExchangeWorker> logger)
        {
            _logger = logger;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_signalrUrl)
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            _hubConnection.StartAsync();

            _hubConnection.On<Guid, String, Double>("CurrencyUpdate", (id, name, value) =>
            {
                _logger.LogInformation($"CurrencyUpdate: {name} [{value:F2}]");
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}