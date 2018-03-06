using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Digipolis.Auth.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digipolis.Auth.Authentication
{
    public class InternalApikeyHeaderAuthenticationHandler : AuthenticationHandler<InternalApikeyHeaderAuthenticationOptions>
    {
        public InternalApikeyHeaderAuthenticationHandler(IOptionsMonitor<InternalApikeyHeaderAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
           
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.Request.Headers.ContainsKey(InternalApikeyHeaderAuthenticationOptions.HEADER_NAME))
            {
                if (Context.Request.Headers[InternalApikeyHeaderAuthenticationOptions.HEADER_NAME] == OptionsMonitor.CurrentValue.HeaderValue)
                {
                    var claims = new ClaimsIdentity(new Claim[] {
                        new Claim(InternalApikeyHeaderAuthenticationOptions.CLAIM_TYPE, OptionsMonitor.CurrentValue.HeaderValue),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "internal-api")
                    }, Scheme.Name);

                    var claimsPrincipal = new ClaimsPrincipal(claims);

                    var authenticationTicket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Fail($"Internal Apikey Header authentication failed"));
                }
            }
            else
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
        }
    }
}