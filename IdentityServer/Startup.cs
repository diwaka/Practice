using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config) {

            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
         
            var connectionString = _config.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseSqlServer(connectionString);
                // config.UseInMemoryDatabase("Memory");
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie"; // identity server cookies you can also see that in the browser.
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });
            var certificate = new X509Certificate2();
            var assembly = typeof(Startup).Assembly.GetName().Name;
            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(assembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(assembly));
                })
                //.AddSigningCredential();
                //.AddInMemoryApiResources(Configuration.GetApiResources())
                //.AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                //.AddInMemoryClients(Configuration.GetClients())
                .AddDeveloperSigningCredential();

            services.AddAuthentication()
                    .AddFacebook(config =>
                    {
                        config.AppId = "543223769808577";
                        config.AppSecret = "6d3870708c33bd9584c446711813407c";
                    })
                    .AddGoogle(config =>
                    {
                        config.ClientId = "566070752713-7o4jr8v75sqetd0ovu44fdv5njqu2frc.apps.googleusercontent.com";
                        config.ClientSecret = "mIcXJChNv30k7AVc2cL51yBx";

                    });
                    //.AddMicrosoftAccount(config=> {
                    //    config.ClientId = "f55aff11-26b9-4176-a857-6b6f28919a7c";
                    //    config.ClientSecret = "O2Ade2BnfhtWcD7rN5C[ctzPL:lhOL_-";
                    //});
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
