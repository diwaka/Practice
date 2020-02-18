using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configuration
    {
        //Two Types of resources one is Apis and another is default scopes

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //"Id_token" => id_token is for authentication.
            // This will contain the information of user which later  consider as id_token 
            //  so basically claims in id_token is get or map it from here.
            List<IdentityResource> resources = new List<IdentityResource>();
            resources.Add(new IdentityResources.OpenId()); // required
            resources.Add(new IdentityResources.Profile()); // required

            resources.Add(new IdentityResource
            {
                Name = "custom_claims",
                UserClaims =  { "Claim.Custom_Name" }

            }) ;

            return resources;
        }

        // "access_token" => access_token is for get the protective resources.
        // This will contain the information of user which later  consider as access_token 
        //  so basically claims in access_token is get or map it from here.
        public static IEnumerable<ApiResource> GetApiResources()
        {

            List<ApiResource> resources = new List<ApiResource>();
            resources.Add(new ApiResource("ApiOne", new string[] { "Claim.Custom_Email" })); // all the claims is going to be share between the all the Api's Resources
            resources.Add(new ApiResource("ApiTwo"));

            return resources;
        }

        public static IEnumerable<Client> GetClients()
        {

            List<Client> clients = new List<Client>();
            clients.Add(new Client
            {
                // Below information to know the clients
                ClientId = "client_id",
                ClientSecrets = { new Secret("client_secrets".ToSha256()) },
                // Way to retreive the Access_token
                AllowedGrantTypes = { GrantType.ClientCredentials },

                // This Clients only able to access the below metioned APIs
                AllowedScopes = { "ApiOne" }

            });

            clients.Add(new Client
            {
                // Below information to know the clients
                ClientId = "client_id_mvc",
                ClientSecrets = { new Secret("client_secrets_mvc".ToSha256()) },
                // Way to retreive the Access_token
                AllowedGrantTypes = { GrantType.AuthorizationCode },

                // redirect back to client's application
                RedirectUris = { "https://localhost:44381/signin-oidc" },

                FrontChannelLogoutUri = "https://localhost:44381/signin-oidc",
                // redirect back to client's application after logout
                PostLogoutRedirectUris = { "https://localhost:44381/Home/Index" },

                // This Clients only able to access the below metioned APIs
                AllowedScopes = {
                    "ApiOne",
                    "ApiTwo",
                    IdentityServerConstants.StandardScopes.OpenId, // add "openid"
                    IdentityServerConstants.StandardScopes.Profile,
                    "custom_claims"
                },

                RequireConsent = false, // when returning back to the client application first check the consent is required on not

                 // put all the claims in id_token

                //AlwaysIncludeUserClaimsInIdToken = true

                AllowOfflineAccess = true, // for refresh token

            });
            clients.Add(new Client
            {
                ClientId = "client_id_js",
                // redirect back to client's application
                RedirectUris = { "https://localhost:44337/Home/SignIn" },

                FrontChannelLogoutUri =  "https://localhost:44337/Home/SignIn",
                // Way to retreive the Access_token + id_token
                AllowedGrantTypes = GrantTypes.Implicit,

                // to resolve the cors error 
                AllowedCorsOrigins = { "https://localhost:44337" },
                // This Clients only able to access the below metioned resources
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId, // add "openid"
                    "ApiOne",
                },

                // redirect back to client's application after logout
                PostLogoutRedirectUris = { "https://localhost:44337/Home/Index" },

                AllowAccessTokensViaBrowser = true,
                RequireConsent = false



            });

            clients.Add(new Client
            {
                 
                ClientId = "client_id_oidc_js",
                // redirect back to client's application
                RedirectUris = { "https://localhost:44337/Home/SignIn" },
                // Way to retreive the Access_token + id_token
                AllowedGrantTypes = GrantTypes.Implicit,
                // to resolve the cors error 
                AllowedCorsOrigins = { "https://localhost:44337" },

                // redirect back to client's application after logout
                PostLogoutRedirectUris = { "https://localhost:44337/Home/Index" },

                // This Clients only able to access the below metioned resources
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId, // add "openid"
                    "ApiOne",
                    "custom_claims"
                },
               // AccessTokenLifetime = 1,
                AllowAccessTokensViaBrowser = true,
           
                RequireConsent = false
                



            });

            return clients;
        }
    }
}
