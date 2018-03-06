using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Digipolis.Auth.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digipolis.Auth.Authentication
{
    public class AProfielAuthenticationHandler : AuthenticationHandler<AProfielAuthenticationOptions>
    {
        public AProfielAuthenticationHandler(IOptionsMonitor<AProfielAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }
        
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.User.Identity.IsAuthenticated && Context.Request.Headers.ContainsKey("Authorization"))
            {
                var claims = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "aanvrager"),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, "aanvrager")
                }, Scheme.Name);
                
                var claimsPrincipal = new ClaimsPrincipal(claims);

                var authenticationTicket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
        }


    }
}