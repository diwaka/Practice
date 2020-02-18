using IdentityServer4.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer4.Config
{
    public class Scopes
    {
        public static List<Scope> GetScopes()
        {

            List<Scope> scopes = new List<Scope>();
            scopes.Add(
                new Scope { });

            return scopes;
        }
    }
}