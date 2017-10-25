using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Digipolis.Auth.Options;
using System.Linq;

namespace Digipolis.Auth.Middleware
{
    public class AProfielAuthenticationHandler : AuthenticationHandler<AProfielAuthenticationOptions>
    {        
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.User.Identity.IsAuthenticated && Context.Request.Headers.ContainsKey("Authorization"))
            {
                var claims = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "aanvrager"),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, "aanvrager")
                }, Options.AuthenticationScheme);
                
                var claimsPrincipal = new ClaimsPrincipal(claims);

                var authenticationTicket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), Options.AuthenticationScheme);
                return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Skip());
            }
        }
    }
}