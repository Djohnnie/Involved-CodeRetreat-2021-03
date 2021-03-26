using System;
using System.Collections.Generic;

namespace InvolvedExchangeWorkers
{
    public interface IExchangeTrend
    {
        void AddCurrencyDetail(CurrencyDetail currencyDetail);

        Guid GetBest();

        Guid GetWorst();
    }

    public class ExchangeTrend : IExchangeTrend
    {
        private readonly Dictionary<Guid, List<CurrencyTrend>> _currencies = new Dictionary<Guid, List<CurrencyTrend>>();

        public void AddCurrencyDetail(CurrencyDetail currencyDetail)
        {
            if (!_currencies.ContainsKey(currencyDetail.Id))
            {
                _currencies.Add(currencyDetail.Id, new List<CurrencyTrend>());
            }

            _currencies[currencyDetail.Id].Add(new CurrencyTrend
            {
                TimeStamp = DateTime.UtcNow,
                CurrencyDetail = currencyDetail
            });
        }

        public Guid GetBest()
        {
            Dictionary<Guid, Decimal> deltas = new Dictionary<Guid, decimal>();

            foreach (var currency in _currencies)
            {
                if (currency.Value.Count >= 10)
                {
                    var delta = currency.Value[currency.Value.Count - 1].CurrencyDetail.Value - currency.Value[currency.Value.Count - 10].CurrencyDetail.Value;
                    deltas.Add(currency.Key, delta);
                }
                else
                {
                    deltas.Add(currency.Key, 0M);
                }
            }

            Decimal delta2 = 0;
            Guid result = Guid.Empty;

            foreach (var x in deltas)
            {
                if (x.Value > delta2)
                {
                    delta2 = x.Value;
                    result = x.Key;
                }
            }

            return result;
        }

        public Guid GetWorst()
        {
            Dictionary<Guid, Decimal> deltas = new Dictionary<Guid, decimal>();

            foreach (var currency in _currencies)
            {
                if (currency.Value.Count >= 10)
                {
                    var delta = currency.Value[currency.Value.Count - 1].CurrencyDetail.Value - currency.Value[currency.Value.Count - 10].CurrencyDetail.Value;
                    deltas.Add(currency.Key, delta);
                }
                else
                {
                    deltas.Add(currency.Key, 0M);
                }
            }

            Decimal delta2 = 0;
            Guid result = Guid.Empty;

            foreach (var x in deltas)
            {
                if (x.Value < delta2)
                {
                    delta2 = x.Value;
                    result = x.Key;
                }
            }

            return result;
        }
    }

    public class CurrencyTrend
    {
        public DateTime TimeStamp { get; set; }

        public CurrencyDetail CurrencyDetail { get; set; }
    }
}