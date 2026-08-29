using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
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
            if (HasStandardError(response) || HasBackendMessage(response) || IsExternalVersionedContract(request) ||
                (int)response.StatusCode < 400) return AddCorsHeaders(request, response);

            string code;
            string message;
            Map(response.StatusCode, out code, out message);

            var statusCode = response.StatusCode;
            response.Dispose();
            return AddCorsHeaders(
                request,
                ApiErrorResponses.Create(request, statusCode, code, message));
        }

        // Web API 2's CORS action filter does not run when routing rejects a
        // request (for example, 404/405) and can also be bypassed by responses
        // replaced in a delegating handler. Apply the configured allowlist to
        // the final response so browser clients can read the real API error.
        private static HttpResponseMessage AddCorsHeaders(
            HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request == null || response == null ||
                !request.Headers.Contains("Origin") ||
                response.Headers.Contains("Access-Control-Allow-Origin"))
                return response;

            var origin = request.Headers.GetValues("Origin").FirstOrDefault();
            var configuredOrigins = ConfigurationManager.AppSettings["AllowedCorsOrigins"]
                ?? "http://localhost:5173";
            var allowedOrigins = configuredOrigins
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
            var allowAnyOrigin = allowedOrigins.Contains("*");

            if (!allowAnyOrigin && !allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                return response;

            response.Headers.TryAddWithoutValidation(
                "Access-Control-Allow-Origin", allowAnyOrigin ? "*" : origin);
            response.Headers.Vary.Add("Origin");
            return response;
        }

        private static bool HasStandardError(HttpResponseMessage response)
        {
            var objectContent = response != null ? response.Content as ObjectContent : null;
            return objectContent != null && objectContent.Value is ApiErrorResponse;
        }

        // Controllers frequently return an anonymous { success = false,
        // message = "..." } body containing the actionable business reason.
        // Keep that response intact. Replacing it with a status-only generic
        // message destroys information that the client cannot recover.
        private static bool HasBackendMessage(HttpResponseMessage response)
        {
            var objectContent = response != null ? response.Content as ObjectContent : null;
            var value = objectContent != null ? objectContent.Value : null;
            if (value == null) return false;

            var messageProperty = value.GetType().GetProperty(
                "Message",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (messageProperty == null) return false;

            var message = messageProperty.GetValue(value, null) as string;
            return !string.IsNullOrWhiteSpace(message);
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
