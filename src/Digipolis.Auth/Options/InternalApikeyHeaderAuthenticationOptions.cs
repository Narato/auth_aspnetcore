using Microsoft.AspNetCore.Builder;

namespace Digipolis.Auth.Options
{
    public class InternalApikeyHeaderAuthenticationOptions : AuthenticationOptions
    {
        public static readonly string HEADER_NAME = "X-internal-apikey";
        public static readonly string CLAIM_TYPE = "internal-apikey";
        
        public string HeaderValue { get; set; }

        public InternalApikeyHeaderAuthenticationOptions()
        {
            this.AuthenticationScheme = "InternalApikeyHeaderAuthentication";
            this.AutomaticAuthenticate = true;
            this.AutomaticChallenge = true;
        }
    }
}