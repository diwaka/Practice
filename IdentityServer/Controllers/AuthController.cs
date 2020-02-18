using IdentityModel;
using IdentityServer.ViewModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;


namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interactionService;
        public AuthController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IIdentityServerInteractionService interactionService,
            IEventService events)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
            _events = events;
        }


        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)

        {
            await _signInManager.SignOutAsync();
           // await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        //[HttpGet]
        //public async Task<IActionResult> SignOut(string logoutId)
        //{
        //    // build a model so the logout page knows what to display
        //    var vm = await BuildLogoutViewModelAsync(logoutId);

        //    if (vm.ShowLogoutPrompt == false)
        //    {
        //        // if the request for logout was properly authenticated from IdentityServer, then
        //        // we don't need to show the prompt and can just log the user out directly.
        //        return await Logout(vm);
        //    }

        //    return View(vm);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Logout(LogoutInputModel model)
        //{
        //    // build a model so the logged out page knows what to display
        //    var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

        //    if (User?.Identity.IsAuthenticated == true)
        //    {
        //        // delete local authentication cookie
        //        await _signInManager.SignOutAsync();

        //        // raise the logout event
        //        await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
        //    }

        //    // check if we need to trigger sign-out at an upstream identity provider
        //    if (vm.TriggerExternalSignout)
        //    {
        //        // build a return URL so the upstream provider will redirect back
        //        // to us after the user has logged out. this allows us to then
        //        // complete our single sign-out processing.
        //        string url = Url.Action("SignOut", new { logoutId = vm.LogoutId });

        //        // this triggers a redirect to the external provider for sign-out
        //        return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
        //    }

        //    return View("LoggedOut", vm);
        //}
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)

        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return View(new LoginViewmodel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewmodel vm)
        {
            var result = await _signInManager.PasswordSignInAsync("Diwakar", "password", false, false);
            if (result.Succeeded)
            {
                return Redirect(vm.ReturnUrl);
            }
            else
            {

            }
            return View();
        }
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var returnUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUri);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info != null)
            {
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

                if (result.Succeeded)
                {
                    return Redirect(returnUrl);
                }
            }
            var username = info.Principal.FindFirst(ClaimTypes.Name.Replace(" ", "_")).Value;
            return View("RegisterExternal", new ExternalProviderViewModel
            {
                UserName = username,
                ReturnUrl = returnUrl
            });
        }

        public async Task<IActionResult> ExternalRegister(ExternalProviderViewModel vm)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var user = new IdentityUser(vm.UserName);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {

                return View("RegisterExternal", vm);
            }
            var addLoginResult = await _userManager.AddLoginAsync(user, info);

            if (!addLoginResult.Succeeded)
            {

                return View("RegisterExternal", vm);
            }

            await _signInManager.SignInAsync(user, false);

            return Redirect(vm.ReturnUrl);
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        //private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        //{
        //    var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = true };

        //    if (User?.Identity.IsAuthenticated != true)
        //    {
        //        // if the user is not authenticated, then just show logged out page
        //        vm.ShowLogoutPrompt = false;
        //        return vm;
        //    }

        //    var context = await _interactionService.GetLogoutContextAsync(logoutId);
        //    if (context?.ShowSignoutPrompt == false)
        //    {
        //        // it's safe to automatically sign-out
        //        vm.ShowLogoutPrompt = false;
        //        return vm;
        //    }

        //    // show the logout prompt. this prevents attacks where the user
        //    // is automatically signed out by another malicious web page.
        //    return vm;
        //}

        //private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        //{
        //    // get context information (client name, post logout redirect URI and iframe for federated signout)
        //    var logout = await _interactionService.GetLogoutContextAsync(logoutId);

        //    var vm = new LoggedOutViewModel
        //    {
        //        AutomaticRedirectAfterSignOut = false,
        //        PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
        //        ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
        //        SignOutIframeUrl = logout?.SignOutIFrameUrl,
        //        LogoutId = logoutId
        //    };

        //    if (User?.Identity.IsAuthenticated == true)
        //    {
        //        var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
        //        if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
        //        {
        //            var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
        //            if (providerSupportsSignout)
        //            {
        //                if (vm.LogoutId == null)
        //                {
        //                    // if there's no current logout context, we need to create one
        //                    // this captures necessary info from the current logged in user
        //                    // before we signout and redirect away to the external IdP for signout
        //                    vm.LogoutId = await _interactionService.CreateLogoutContextAsync();
        //                }

        //                vm.ExternalAuthenticationScheme = idp;
        //            }
        //        }
        //    }

        //    return vm;
        //}
    }
}

