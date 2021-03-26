using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvolvedExchangeWorkers
{
    public interface IApiClient
    {
        Task<string> Authenticate();
        Task<GetPortfolioResponse> GetPortfolio(string token);
        Task<List<CurrencyDetail>> GetCurrencies(string token);
        Task BuyCurrency(string token, BuyCurrencyRequest body);
    }

    public class ApiClient : IApiClient
    {
        private readonly string _username = "annny";
        private readonly string _password = "AnteJohnny!";

        private readonly string _baseUrl = "https://involvedexchangewebapi20210324153741.azurewebsites.net/api";
        private readonly string _authenticateRoute = "User/Authenticate";
        private readonly string _getPortfolioRoute = "Account/GetPortfolio";
        private readonly string _getCurrenciesRoute = "Currency/GetCurrencies";
        private readonly string _buyCurrencyRoute = "Account/BuyCurrency";

        public async Task<string> Authenticate()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(_authenticateRoute, Method.POST);
            request.AddJsonBody(new AuthenticateRequest
            {
                Username = _username,
                Password = _password
            });
            var response = await client.ExecuteAsync<AuthenticateResponse>(request);

            return response.Data.Token;
        }

        public async Task<GetPortfolioResponse> GetPortfolio(string token)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(_getPortfolioRoute, Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = await client.ExecuteAsync<GetPortfolioResponse>(request);

            return response.Data;
        }

        public async Task<List<CurrencyDetail>> GetCurrencies(string token)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(_getCurrenciesRoute, Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = await client.ExecuteAsync<List<CurrencyDetail>>(request);

            return response.Data;
        }

        public async Task BuyCurrency(string token, BuyCurrencyRequest body)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(_buyCurrencyRoute, Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(body);
            _ = await client.ExecuteAsync(request);
        }
    }

    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticateResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }

    public class GetPortfolioResponse
    {
        public List<Currency> Currencies { get; set; }
    }

    public class Currency
    {
        public Guid CurrencyId { get; set; }
        public Decimal Amount { get; set; }
        public Decimal Value { get; set; }
    }

    public class CurrencyDetail
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public Decimal Value { get; set; }
    }

    public class BuyCurrencyRequest
    {
        public Guid FromCurrencyId { get; set; }
        public Guid ToCurrencyId { get; set; }
        public Decimal FromAmount { get; set; }
        public Decimal ToAmount { get; set; }
    }
}