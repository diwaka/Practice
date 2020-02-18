using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer4.Config
{
    public class Clients
    {
        public static IEnumerable<ApiResource> Apis =>
         new List<ApiResource>
         {
            new ApiResource("api1", "My API")
         };
        public static List<Client> GetClients()
        {


            List<Client> clients = new List<Client>();

            clients.Add(new Client
            {
                ClientName = "IdentityServer4",
                ClientId = "Frontend",
                ClientSecrets = {
                new Secret("Frontend".Sha256())
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = new List<string>
                {
                  "IdentityServer4.Api"
                }

            }); ;

            return clients;
        }
    }
}