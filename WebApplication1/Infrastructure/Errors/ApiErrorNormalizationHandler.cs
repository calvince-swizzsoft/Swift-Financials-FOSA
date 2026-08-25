using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.ApiErrors
{
    public sealed class ApiErrorNormalizationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (HasStandardError(response)) return response;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                return ApiErrorResponses.Create(request, HttpStatusCode.Unauthorized,
                    ErrorCodes.AuthenticationRequired, "Authentication is required.");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                response.Dispose();
                return ApiErrorResponses.Create(request, HttpStatusCode.Forbidden,
                    ErrorCodes.AccessDenied, "You do not have permission to perform this operation.");
            }

            return response;
        }

        private static bool HasStandardError(HttpResponseMessage response)
        {
            var objectContent = response != null ? response.Content as ObjectContent : null;
            return objectContent != null && objectContent.Value is ApiErrorResponse;
        }
    }
}
