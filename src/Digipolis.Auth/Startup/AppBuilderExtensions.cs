﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Digipolis.Auth.Jwt;
using Digipolis.Auth.Options;
using Digipolis.Auth.PDP;
using Digipolis.Auth.Middleware;

namespace Digipolis.Auth
{
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Adds Authentication and Authorization to the Microsoft.AspNetCore.Builder.IApplicationBuilder request execution pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            var authOptions = app.ApplicationServices.GetService<IOptions<AuthOptions>>().Value;
            var tokenRefreshHandler = app.ApplicationServices.GetService<ITokenRefreshHandler>();
            var jwtBearerOptionsFactory = app.ApplicationServices.GetService<JwtBearerOptionsFactory>();

            if (authOptions.EnableInternalApikeyHeaderAuth)
            {
                app.UseInternalKeyHeaderAuth();
            }

            var jwtBearerOptions = jwtBearerOptionsFactory.Create();
            jwtBearerOptions.AuthenticationScheme = AuthSchemes.JwtHeaderAuth;

            if (authOptions.EnableJwtHeaderAuth)
            {
                // Add middleware that handles jwt tokens present in Authentication Header
                app.UseJwtBearerAuthentication(jwtBearerOptions);
            }

            if (authOptions.EnableAProfielAuth)
            {
                // IMPORTANT: temporary workaround to enable authentication for A-Profiel
                // This must be replaced with an OAuth validation which checks the AProfiel access token
                app.UseAProfielAuth();
            }

            if (authOptions.EnableCookieAuth)
            {
                //Add middleware that handles authentication cookie
                var cookieAuthOptions = new CookieAuthenticationOptions
                {
                    AuthenticationScheme = AuthSchemes.CookieAuth,

                    AccessDeniedPath = new PathString($"/{authOptions.AccessDeniedPath}"),
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,

                    Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = async context =>
                        {
                            if (authOptions.AutomaticTokenRefresh)
                            {
                                var token = context.Request.Cookies["jwt"];

                                var response = await tokenRefreshHandler.HandleRefreshAsync(token);

                                if (response != null)
                                    context.Response.Cookies.Append("jwt", response);
                            }
                        },

                        OnRedirectToAccessDenied = context =>
                        {
                            if (IsAjaxRequest(context.Request))
                            {
                                context.Response.Headers["Location"] = context.RedirectUri;
                                context.Response.StatusCode = 403;
                            }
                            else
                            {
                                context.Response.Redirect(new PathString($"/{authOptions.AccessDeniedPath}"));
                            }
                            return Task.FromResult<object>(null);
                        },

                        OnRedirectToLogin = context =>
                        {
                            if (IsAjaxRequest(context.Request))
                            {
                                context.Response.Headers["Location"] = context.RedirectUri;
                                context.Response.StatusCode = 401;
                            }
                            else
                            {
                                var url = $"{authOptions.ApiAuthUrl}?idp_url={authOptions.ApiAuthIdpUrl}&sp_name={authOptions.ApiAuthSpName}&sp_url={authOptions.ApiAuthSpUrl}&client_redirect={authOptions.ApplicationBaseUrl}/{authOptions.TokenCallbackRoute}?returnUrl=";

                                context.RedirectUri = Uri.EscapeUriString(url + context.Request.Path);
                                context.Response.Redirect(context.RedirectUri);
                            }
                            return Task.FromResult<object>(null);
                        },
                    }
                };

                app.UseCookieAuthentication(cookieAuthOptions);
            }

            //Add middleware to set permissions in user claims
            var claimsTransformer = app.ApplicationServices.GetService<PermissionsClaimsTransformer>();
            app.UseClaimsTransformation(new ClaimsTransformationOptions
            {
                Transformer = claimsTransformer
            });

            return app;
        }

        public static IApplicationBuilder UseInternalKeyHeaderAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InternalApikeyHeaderAuthenticationMiddleware>();
        }
        public static IApplicationBuilder UseAProfielAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AProfielAuthenticationMiddleware>();
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return (request.Path.StartsWithSegments(new PathString("/api"), StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
                
        }
    }
}
