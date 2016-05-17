﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Toolbox.Auth.Options;
using Microsoft.AspNetCore.Builder;
using System;

namespace Toolbox.Auth.Jwt
{
    internal class JwtBearerOptionsFactory
    {
        public static JwtBearerOptions Create(AuthOptions authOptions, IJwtSigningKeyProvider signingKeyProvider, IJwtTokenSignatureValidator signatureValidator,
            ILogger<JwtBearerMiddleware> logger)
        {
            var jwtBearerOptions = new JwtBearerOptions
            {
                AutomaticAuthenticate = true
            };

            jwtBearerOptions.TokenValidationParameters = TokenValidationParametersFactory.Create(authOptions, signatureValidator);

            jwtBearerOptions.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    logger.LogInformation($"Jwt token validation failed. Exception: {context.Exception.ToString()}");

                    context.Ticket = new Microsoft.AspNetCore.Authentication.AuthenticationTicket(new ClaimsPrincipal(), new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties(), string.Empty);
                    context.HandleResponse();

                    return Task.FromResult<object>(null);
                },
                OnChallenge = context =>
                {
                    return Task.FromResult<object>(null);
                },
                OnMessageReceived = async context =>
                {
                    //the signingKey is resolved on this event because we can make the call async here, in the signatureValidator async is not possible
                    if (!String.IsNullOrWhiteSpace(authOptions.JwtSigningKeyProviderUrl))
                        context.Options.TokenValidationParameters.IssuerSigningKey = await signingKeyProvider.ResolveSigningKeyAsync(true);
                }
            };

            return jwtBearerOptions;
        }
    }
}
