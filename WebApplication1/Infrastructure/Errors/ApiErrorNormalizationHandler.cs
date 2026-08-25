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
            if (HasStandardError(response) || IsExternalVersionedContract(request) ||
                (int)response.StatusCode < 400) return response;

            string code;
            string message;
            Map(response.StatusCode, out code, out message);

            var statusCode = response.StatusCode;
            response.Dispose();
            return ApiErrorResponses.Create(request, statusCode, code, message);
        }

        private static bool HasStandardError(HttpResponseMessage response)
        {
            var objectContent = response != null ? response.Content as ObjectContent : null;
            return objectContent != null && objectContent.Value is ApiErrorResponse;
        }

        private static bool IsExternalVersionedContract(HttpRequestMessage request)
        {
            var path = request != null && request.RequestUri != null
                ? request.RequestUri.AbsolutePath
                : string.Empty;
            return path.StartsWith("/v1/", System.StringComparison.OrdinalIgnoreCase);
        }

        private static void Map(HttpStatusCode statusCode, out string code, out string message)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest:
                    code = ErrorCodes.InvalidRequest;
                    message = "The request is invalid.";
                    return;
                case HttpStatusCode.Unauthorized:
                    code = ErrorCodes.AuthenticationRequired;
                    message = "Authentication is required.";
                    return;
                case HttpStatusCode.Forbidden:
                    code = ErrorCodes.AccessDenied;
                    message = "You do not have permission to perform this operation.";
                    return;
                case HttpStatusCode.NotFound:
                    code = ErrorCodes.ResourceNotFound;
                    message = "The requested resource was not found.";
                    return;
                case HttpStatusCode.MethodNotAllowed:
                    code = ErrorCodes.MethodNotAllowed;
                    message = "The HTTP method is not supported for this resource.";
                    return;
                case HttpStatusCode.NotAcceptable:
                    code = ErrorCodes.NotAcceptable;
                    message = "The requested response format is not supported.";
                    return;
                case HttpStatusCode.Conflict:
                    code = ErrorCodes.ResourceConflict;
                    message = "The request conflicts with the resource's current state.";
                    return;
                case HttpStatusCode.RequestEntityTooLarge:
                    code = ErrorCodes.PayloadTooLarge;
                    message = "The request payload is too large.";
                    return;
                case HttpStatusCode.UnsupportedMediaType:
                    code = ErrorCodes.UnsupportedMediaType;
                    message = "The request content type is not supported.";
                    return;
                case (HttpStatusCode)429:
                    code = ErrorCodes.RateLimitExceeded;
                    message = "Too many requests were submitted. Try again later.";
                    return;
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    code = ErrorCodes.DependencyUnavailable;
                    message = "A required service is temporarily unavailable.";
                    return;
                default:
                    code = (int)statusCode >= 500 ? ErrorCodes.InternalError : ErrorCodes.InvalidRequest;
                    message = (int)statusCode >= 500
                        ? "An unexpected error occurred."
                        : "The request could not be completed.";
                    return;
            }
        }
    }
}
