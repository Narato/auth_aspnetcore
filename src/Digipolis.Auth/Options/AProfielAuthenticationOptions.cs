using Microsoft.AspNetCore.Builder;

namespace Digipolis.Auth.Options
{
    public class AProfielAuthenticationOptions : AuthenticationOptions
    {
        public AProfielAuthenticationOptions()
        {
            this.AuthenticationScheme = "AProfielAuthentication";
            this.AutomaticAuthenticate = true;
            this.AutomaticChallenge = true;
        }
    }
}