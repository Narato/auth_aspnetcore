using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Digipolis.Auth.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digipolis.Auth.Middleware
{
  public class AProfielAuthenticationMiddleware : AuthenticationMiddleware<AProfielAuthenticationOptions>
  {
    private readonly RequestDelegate _next;
    
    public AProfielAuthenticationMiddleware(RequestDelegate next, IOptions<AProfielAuthenticationOptions> options, 
                                              ILoggerFactory loggerFactory, UrlEncoder encoder) : base(next, options, loggerFactory, encoder)
    {
        _next = next;
    }

    protected override AuthenticationHandler<AProfielAuthenticationOptions> CreateHandler()
    {
        return new AProfielAuthenticationHandler();
    }
  }
}