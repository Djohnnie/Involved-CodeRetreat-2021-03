using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvolvedExchangeWorkers
{
    public class SaleWorker : BackgroundService
    {
        private readonly IApiClient _apiClient;
        private readonly IExchangeTrend _exchangeTrend;
        private readonly ILogger<SaleWorker> _logger;

        public SaleWorker(
            IApiClient apiClient,
            IExchangeTrend exchangeTrend,
            ILogger<SaleWorker> logger)
        {
            _apiClient = apiClient;
            _exchangeTrend = exchangeTrend;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var token = await _apiClient.Authenticate();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var portfolio = await _apiClient.GetPortfolio(token);
                    var currencies = await _apiClient.GetCurrencies(token);

                    var euroId = currencies.Single(x => x.Name == "Euro");

                    var bestCurrencyId = _exchangeTrend.GetBest();
                    var worstCurrencyId = _exchangeTrend.GetWorst();

                    if (bestCurrencyId != Guid.Empty && worstCurrencyId != Guid.Empty)
                    {
                        var worstCurrency = portfolio.Currencies.Single(x => x.CurrencyId == worstCurrencyId);

                        if( worstCurrency.Amount == 0M)
                        {
                            worstCurrency = portfolio.Currencies.Single(x => x.CurrencyId == euroId.Id);
                            worstCurrencyId = worstCurrency.CurrencyId;
                        }

                        var buy = new BuyCurrencyRequest
                        {
                            FromCurrencyId = worstCurrencyId,
                            ToCurrencyId = bestCurrencyId,
                            FromAmount = worstCurrency.Amount / 3M
                        };

                        await _apiClient.BuyCurrency(token, buy);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
