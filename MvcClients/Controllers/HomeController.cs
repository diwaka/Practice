using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace MvcClients.Controllers
{
    public class HomeController : Controller
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Secret()
        {

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var claims = User.Claims;
            var access_token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var id_token = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            //var refresh_token = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
        //    await RefreshAccessToken();
            var result = await GetFirstApi(accessToken);
            return View();
        }

        public async Task<string> GetFirstApi(string accessToken)
        {
            //retrieve secret data;
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var response = await apiClient.GetAsync("https://localhost:44324/secret");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task RefreshAccessToken()
        {

            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44399/");

            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "client_id_mvc",
                ClientSecret = "client_secrets_mvc"
            });

            var authInfo = await HttpContext.AuthenticateAsync("Cookie"); // same as configure in Startup.cs
            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("Cookie", authInfo.Principal,authInfo.Properties);


        }
        public IActionResult SignOut()
        {
            return SignOut("Cookie", "oidc");
        }
    }
}
