using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvolvedExchangeWorkers
{
    public interface IApiClient
    {
        Task<string> Authenticate();
    }

    public class ApiClient : IApiClient
    {
        private readonly string _username = "annny";
        private readonly string _password = "AnteJohnny!";

        private readonly string _baseUrl = "https://involvedexchangewebapi20210324153741.azurewebsites.net/api";
        private readonly string _authenticateRoute = "User/Authenticate";

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
}