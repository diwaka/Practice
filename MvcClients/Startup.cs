using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClients
{
    public class Startup
    {
       /** https://openid.net/specs/openid-connect-core-1_0.html **/

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config => {
                config.DefaultScheme = "Cookie";
                config.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookie") // later you can see this cookie in the browser (same name as you wrote here )
                .AddOpenIdConnect("oidc",config=> {
                    config.Authority = "https://localhost:44399/"; // server 
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secrets_mvc";
                    config.SaveTokens = true; // Telling save token in our Cookies
                    config.ResponseType = "code";
                    config.SignedOutCallbackPath = "/Home/Index";
                    // Configure cookie claim mapping
                    config.ClaimActions.MapUniqueJsonKey("Clients_Cookies", "Claim.Custom_Name");

                    config.GetClaimsFromUserInfoEndpoint = true; // two trips to load claims into the cookies but the id_token is smaller
                    config.Scope.Add("custom_claims");
                    config.Scope.Add("ApiOne"); // if you omit this line you will not going to access Api from project Apis
                    config.Scope.Add("offline_access"); // In order to get refresh_token you need to request for it by adding the scope offline_access. but in implicit flow you are not allow to refresh the token.
                });
            /**
             * Mainly 3 types of flow 
             * 1.Authorize code Flow
             * 2.Implicit Flow
             * 3.Hybrid Flow -- rearly use -- use mainly in banking system 
             **/

            services.AddHttpClient();
            services.AddControllersWithViews();
        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
