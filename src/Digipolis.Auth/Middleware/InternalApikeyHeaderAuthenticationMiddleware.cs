using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Digipolis.Auth.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Digipolis.Auth.Middleware
{
  public class InternalApikeyHeaderAuthenticationMiddleware : AuthenticationMiddleware<InternalApikeyHeaderAuthenticationOptions>
  {
    private readonly RequestDelegate _next;
    
    public InternalApikeyHeaderAuthenticationMiddleware(RequestDelegate next, IOptions<InternalApikeyHeaderAuthenticationOptions> options, 
                                              ILoggerFactory loggerFactory, UrlEncoder encoder) : base(next, options, loggerFactory, encoder)
    {
        _next = next;
    }

    protected override AuthenticationHandler<InternalApikeyHeaderAuthenticationOptions> CreateHandler()
    {
        return new InternalApikeyHeaderAuthenticationHandler();
    }
  }
}