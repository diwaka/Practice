using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ApisTwo.Controllers
{
    // This ApiTwo is currently behaving as a Clients
    public class HomeController : Controller
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        [Route("/home")]
        public async Task<IActionResult> Index()
        {
            // retrieve access_token;
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44399/");
            var token = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secrets",
                    Scope = "ApiOne"

                }
                );
            //retrieve secret data;
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(token.AccessToken);
            var response = await apiClient.GetAsync("https://localhost:44324/secret");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(new
            {
                access_token = token.AccessToken,
                message = content
            }) ;
        }
    }
}
