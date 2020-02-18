using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.ViewModel
{
    public class ExternalProviderViewModel
    {
        public string UserName { get; set; }
        public string ReturnUrl { get; set; }
    }
}
