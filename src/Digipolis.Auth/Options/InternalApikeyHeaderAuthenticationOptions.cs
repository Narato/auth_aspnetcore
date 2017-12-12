using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Digipolis.Auth.Options
{
    public class InternalApikeyHeaderAuthenticationOptions : AuthenticationSchemeOptions
    {
        public static readonly string HEADER_NAME = "X-internal-apikey";
        public static readonly string CLAIM_TYPE = "internal-apikey";
        
        public string HeaderValue { get; set; }
    }
}