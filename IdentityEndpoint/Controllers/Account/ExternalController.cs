using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HundredProof.Federation.DataModel.UserDatabase;
using HundredProof.Federation.Domain;
using HundredProof.Federation.Domain.Account;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityEndpoint.Controllers.Account {
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller {
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<ExternalController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _logger = logger;
        }

        /// <summary>
        ///     initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string provider, string returnUrl) {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties {
                RedirectUri = Url.Action(nameof(Callback)),
                Items = {
                    {"returnUrl", returnUrl},
                    {"scheme", provider}
                }
            };

            return Challenge(props, provider);
        }

        /// <summary>
        ///     Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback() {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result?.Succeeded != true) throw new Exception("External authentication error");

            if (_logger.IsEnabled(LogLevel.Debug)) {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }
            var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);
            if (user == null)
                user = await AutoProvisionUserAsync(provider, providerUserId, claims);
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);
            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id;
            await HttpContext.SignInAsync(user.Id, name, provider, localSignInProps, additionalLocalClaims.ToArray());
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name, true,
                context?.ClientId));
            if (context != null)
                if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    return View("Redirect", new RedirectViewModel {RedirectUrl = returnUrl});

            return Redirect(returnUrl);
        }

        private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result) {
            var externalUser = result.Principal;
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);
            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);
            return (user, provider, providerUserId, claims);
        }

        private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId,
            IEnumerable<Claim> claims) {
            var filtered = new List<Claim>();
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                       claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null) {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                            claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                           claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                else if (first != null)
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                else if (last != null) filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }

            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
                        claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email != null) filtered.Add(new Claim(JwtClaimTypes.Email, email));

            var user = new ApplicationUser {
                UserName = Guid.NewGuid().ToString()
            };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any()) {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult =
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }


        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps) {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null) localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
                localSignInProps.StoreTokens(new[] {new AuthenticationToken {Name = "id_token", Value = id_token}});
        }

        private void ProcessLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps) { }

        private void ProcessLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps) { }
    }
}