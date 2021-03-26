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
    }

    public class ApiClient : IApiClient
    {
        private readonly string _username = "annny";
        private readonly string _password = "AnteJohnny!";

        private readonly string _baseUrl = "https://involvedexchangewebapi20210324153741.azurewebsites.net/api";
        private readonly string _authenticateRoute = "User/Authenticate";
        private readonly string _getPortfolioRoute = "Account/GetPortfolio";

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
}