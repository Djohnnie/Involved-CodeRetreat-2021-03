using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvolvedExchangeWorkers
{
    public class Worker : BackgroundService
    {
        private readonly HubConnection _hubConnection;

        private readonly ILogger<Worker> _logger;
        private readonly IApiClient _apiClient;

        public Worker(
            IApiClient apiClient,
            ILogger<Worker> logger)
        {
            _apiClient = apiClient;
            _logger = logger;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://involvedexchangewebapi20210324153741.azurewebsites.net/InvolvedExchange")
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hubConnection.StartAsync(stoppingToken);
                _hubConnection.On<Guid, String, Double>("CurrencyUpdate", (id, name, value) =>
                {
                    _logger.LogInformation($"CurrencyUpdate: {name} [{value:F2}]");
                });

                var token = await _apiClient.Authenticate();
                var response = await _apiClient.GetPortfolio(token);
                var currencies = await _apiClient.GetCurrencies(token);

                _logger.LogInformation(token);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
