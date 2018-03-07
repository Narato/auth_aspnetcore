using System;
using Digipolis.Auth.Authentication;
using Digipolis.Auth.Authorization;
using Digipolis.Auth.Options;
using Microsoft.AspNetCore.Authentication;

namespace Digipolis.Auth
{
    public static class CustomAuthExtensions
    {
        public static AuthenticationBuilder AddInternalAPIKeyHeaderAuthentication(this AuthenticationBuilder builder, Action<InternalApikeyHeaderAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<InternalApikeyHeaderAuthenticationOptions, InternalApikeyHeaderAuthenticationHandler>(AuthSchemes.InternalAPIAuth, "Internal api Auth", configureOptions);
        }
    }
}