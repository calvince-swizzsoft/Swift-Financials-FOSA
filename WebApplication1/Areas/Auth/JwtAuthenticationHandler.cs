using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebApplication1.Areas.Admin.Services;

namespace WebApplication1.Areas.Auth
{
    public class JwtAuthenticationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeader = request.Headers.Authorization;

            if (authHeader != null && authHeader.Scheme == "Bearer" && !string.IsNullOrWhiteSpace(authHeader.Parameter))
            {
                try
                {
                    var principal = JwtTokenService.ValidateToken(authHeader.Parameter);
                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null) HttpContext.Current.User = principal;
                    request.GetRequestContext().Principal = principal;
                }
                catch
                {
                    return request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid or expired token");
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

    }
}