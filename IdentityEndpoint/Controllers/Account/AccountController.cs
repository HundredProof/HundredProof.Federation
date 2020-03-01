using System;
using System.Linq;
using System.Threading.Tasks;
using HundredProof.Federation.DataModel.UserDatabase;
using HundredProof.Federation.Domain;
using HundredProof.Federation.Domain.Account;
using HundredProof.Federation.Domain.LegacySqlLoginAdapter;
using IdentityEndpoint.DataModels;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdentityEndpoint.Controllers.Account {
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller {
        private readonly IClientStore _clientStore;
        private readonly IConfiguration _configuration;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILegacySqlLoginAdapter<SqlUserModel> _legacySqlLoginAdapter;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IConfiguration configuration,
            ILegacySqlLoginAdapter<SqlUserModel> legacySqlLoginAdapter) {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _configuration = configuration;
            _legacySqlLoginAdapter = legacySqlLoginAdapter;
        }

        /// <summary>
        ///     Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl) {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly) 
                return RedirectToAction("Challenge", "External", new {provider = vm.ExternalLoginScheme, returnUrl});

            return View(vm);
        }

        /// <summary>
        ///     Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button) {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (ModelState.IsValid) {
                SqlUserModel sqlUser;
                _legacySqlLoginAdapter.AddCredentials(model.Username, model.Password);
                if (await _userManager.FindByNameAsync(model.Username) == null) {
                    if (_legacySqlLoginAdapter.CheckUser()) {
                        sqlUser = _legacySqlLoginAdapter.LoginWithUser();
                        if (sqlUser != null) {
                            var appUser = new ApplicationUser {
                                Email = sqlUser.Email,
                                UserName = sqlUser.Username,
                                EmailConfirmed = true,
                                NormalizedEmail = sqlUser.Email.ToUpper(),
                                NormalizedUserName = sqlUser.Username.ToUpper()
                            };
                            var user = await _userManager.CreateAsync(appUser);
                            await _userManager.AddLoginAsync(appUser,
                                new UserLoginInfo("legacySql", "legacySql", "legacySqlProvider"));

                            await _signInManager.SignInAsync(appUser, new AuthenticationProperties(), "legacySql");
                            await _userManager.AddClaimsAsync(appUser, sqlUser.Claims);
                            await _events.RaiseAsync(new UserLoginSuccessEvent(appUser.UserName, appUser.Id,
                                appUser.UserName, clientId: context?.ClientId));
                            if (context != null) {
                                if (await _clientStore.IsPkceClientAsync(context.ClientId))
                                    // if the client is PKCE then we assume it's native, so this change in how to
                                    // return the response is for better UX for the end user.
                                    return View("Redirect", new RedirectViewModel {RedirectUrl = model.ReturnUrl});

                                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                                return Redirect(model.ReturnUrl);
                            }

                            // request for a local page
                            if (Url.IsLocalUrl(model.ReturnUrl))
                                return Redirect(model.ReturnUrl);
                            if (string.IsNullOrEmpty(model.ReturnUrl))
                                return Redirect("~/");
                            throw new Exception("invalid return URL");
                        }
                    }
                }

                var sqlLogin = _legacySqlLoginAdapter.LoginWithUser();
                if (sqlLogin != null) {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _signInManager.SignInAsync(user, null, "legacySql");
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName,
                        clientId: context?.ClientId));

                    if (context != null) {
                        if (await _clientStore.IsPkceClientAsync(context.ClientId))
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new RedirectViewModel {RedirectUrl = model.ReturnUrl});

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    if (string.IsNullOrEmpty(model.ReturnUrl))
                        return Redirect("~/");
                    throw new Exception("invalid return URL");
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials",
                    clientId: context?.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }


        /// <summary>
        ///     Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId) {
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
                return await Logout(vm);

            return View(vm);
        }

        /// <summary>
        ///     Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model) {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true) {
                await _signInManager.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }
            if (vm.TriggerExternalSignout) {
                var url = Url.Action("Logout", new {logoutId = vm.LogoutId});
                return SignOut(new AuthenticationProperties {RedirectUri = url}, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied() {
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl) {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null) {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;
                var vm = new LoginViewModel {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint
                };

                if (!local) vm.ExternalProviders = new[] {new ExternalProvider {AuthenticationScheme = context.IdP}};

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Select(x => new ExternalProvider {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null) {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null) {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                        providers = providers.Where(provider =>
                            client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }

            return new LoginViewModel {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model) {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId) {
            var vm = new LogoutViewModel {LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt};

            if (User?.Identity.IsAuthenticated != true) {
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false) {
                vm.ShowLogoutPrompt = false;
                return vm;
            }
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId) {
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true) {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider) {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout) {
                        if (vm.LogoutId == null)
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}