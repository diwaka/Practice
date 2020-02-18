using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using IdentityServer4.Config;
using Microsoft.AspNetCore.Builder;

[assembly: OwinStartup(typeof(IdentityServer4.Startup))]

namespace IdentityServer4
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            //services.AddControllersWithViews();

            var builder = services.AddIdentityServer()
                .AddInMemoryApiResources(Clients.Apis)
                .AddInMemoryClients(Clients.GetClients());
        }
        public void Configuration(IApplicationBuilder app)
        {
           
        }
    }
}
