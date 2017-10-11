using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Digipolis.Auth.Options;

namespace Digipolis.Auth.Middleware
{
    public class InternalApikeyHeaderAuthenticationHandler : AuthenticationHandler<InternalApikeyHeaderAuthenticationOptions>
    {        
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.Request.Headers.ContainsKey(InternalApikeyHeaderAuthenticationOptions.HEADER_NAME))
            {
                if (Context.Request.Headers[InternalApikeyHeaderAuthenticationOptions.HEADER_NAME] == Options.HeaderValue)
                {
                    var claims = new ClaimsIdentity(new Claim[] {
                        new Claim(InternalApikeyHeaderAuthenticationOptions.CLAIM_TYPE, Options.HeaderValue),
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "internal-api")
                    }, Options.AuthenticationScheme);
                    
                    var claimsPrincipal = new ClaimsPrincipal(claims);

                    var authenticationTicket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), Options.AuthenticationScheme);
                    return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Fail($"Internal Apikey Header authentication failed"));
                }
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Skip());
            }
        }
    }
}