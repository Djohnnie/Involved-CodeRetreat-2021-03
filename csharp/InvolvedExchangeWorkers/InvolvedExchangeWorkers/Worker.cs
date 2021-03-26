using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvolvedExchangeWorkers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IApiClient _apiClient;

        public Worker(
            IApiClient apiClient,
            ILogger<Worker> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await _hubConnection.StartAsync(stoppingToken);
                //_hubConnection.On<Guid, String, Double>("CurrencyUpdate", (id, name, value) =>
                //{
                //    _logger.LogInformation($"CurrencyUpdate: {name} [{value:F2}]");
                //});

                var token = await _apiClient.Authenticate();
                var response = await _apiClient.GetPortfolio(token);

                //_logger.LogInformation($"{response.Currencies.Select(x => x.Value).Sum()}");

                //var currencies = await _apiClient.GetCurrencies(token);

                //var euroCurrency = currencies.Single(x => x.Name == "Euro");

                //foreach (var currency in currencies)
                //{
                //    if (currency.Id != euroCurrency.Id)
                //    {
                //        var buy = new BuyCurrencyRequest
                //        {
                //            FromCurrencyId = euroCurrency.Id,
                //            ToCurrencyId = currency.Id,
                //            FromAmount = 250M
                //        };

                //        await _apiClient.BuyCurrency(token, buy);
                //    }
                //}

                //_logger.LogInformation(token);

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
