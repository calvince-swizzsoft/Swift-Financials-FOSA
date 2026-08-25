using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.ApiErrors;

namespace WebApplication1.ErrorHandling.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                Run().GetAwaiter().GetResult();
                Console.WriteLine("All API error-handling tests passed.");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                return 1;
            }
        }

        private static async Task Run()
        {
            await StandardEnvelopeUsesDocumentedShape();
            await ClassifiedExceptionKeepsStatusAndCode();
            await UnexpectedExceptionIsSanitized();
            await ValidCorrelationIdIsPreserved();
            await InvalidCorrelationIdIsReplaced();
            await FrameworkUnauthorizedResponseIsNormalized();
            await FrameworkNotFoundResponseIsNormalized();
            await ExternalVersionedErrorIsNotRewritten();
        }

        private static async Task StandardEnvelopeUsesDocumentedShape()
        {
            var request = NewRequest();
            var validation = new Dictionary<string, string[]>
            {
                { "userName", new[] { "Username is required." } }
            };
            var response = ApiErrorResponses.Create(request, HttpStatusCode.BadRequest,
                ErrorCodes.ValidationFailed, "One or more fields are invalid.", validation);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            Equal(false, (bool)json["success"], "success");
            Equal("VALIDATION_FAILED", (string)json["error"]["code"], "error.code");
            Equal("Username is required.", (string)json["error"]["validationErrors"]["userName"][0],
                "validationErrors.userName");
            Present((string)json["correlationId"], "correlationId");
            Equal((string)json["correlationId"], Header(response), "correlation header");
        }

        private static async Task ClassifiedExceptionKeepsStatusAndCode()
        {
            var response = ApiExceptionHandler.CreateResponse(NewRequest(),
                new BusinessConflictException("BATCH_NOT_AUDITED", "The batch must be audited first."));
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            Equal(HttpStatusCode.Conflict, response.StatusCode, "classified status");
            Equal("BATCH_NOT_AUDITED", (string)json["error"]["code"], "classified code");
            Equal("The batch must be audited first.", (string)json["message"], "safe message");
        }

        private static async Task UnexpectedExceptionIsSanitized()
        {
            const string secret = "SQL password=do-not-return";
            var response = ApiExceptionHandler.CreateResponse(NewRequest(), new InvalidOperationException(secret));
            var body = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(body);

            Equal(HttpStatusCode.InternalServerError, response.StatusCode, "unexpected status");
            Equal(ErrorCodes.InternalError, (string)json["error"]["code"], "unexpected code");
            False(body.Contains(secret), "unexpected response leaked exception details");
        }

        private static async Task ValidCorrelationIdIsPreserved()
        {
            const string expected = "client-request-123";
            var request = NewRequest();
            request.Headers.Add(CorrelationIdHandler.HeaderName, expected);
            var response = await SendThroughCorrelationHandler(request);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            Equal(expected, Header(response), "valid correlation header");
            Equal(expected, (string)json["correlationId"], "valid correlation body");
        }

        private static async Task InvalidCorrelationIdIsReplaced()
        {
            var request = NewRequest();
            request.Headers.TryAddWithoutValidation(CorrelationIdHandler.HeaderName, "invalid correlation id with spaces");
            var response = await SendThroughCorrelationHandler(request);
            var actual = Header(response);

            Present(actual, "replacement correlation header");
            False(actual.Contains(" "), "invalid correlation ID was preserved");
        }

        private static async Task FrameworkUnauthorizedResponseIsNormalized()
        {
            var request = NewRequest();
            var handler = new CorrelationIdHandler
            {
                InnerHandler = new ApiErrorNormalizationHandler
                {
                    InnerHandler = new StatusResponseHandler(HttpStatusCode.Unauthorized)
                }
            };

            HttpResponseMessage response;
            using (var invoker = new HttpMessageInvoker(handler))
                response = await invoker.SendAsync(request, CancellationToken.None);

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            Equal(ErrorCodes.AuthenticationRequired, (string)json["error"]["code"],
                "normalized unauthorized code");
            Equal((string)json["correlationId"], Header(response),
                "normalized unauthorized correlation ID");
        }

        private static async Task FrameworkNotFoundResponseIsNormalized()
        {
            var response = await SendThroughNormalizer(NewRequest(), HttpStatusCode.NotFound);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            Equal(ErrorCodes.ResourceNotFound, (string)json["error"]["code"],
                "normalized not-found code");
        }

        private static async Task ExternalVersionedErrorIsNotRewritten()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/v1/accounts/balance");
            request.SetConfiguration(new HttpConfiguration());
            var response = await SendThroughNormalizer(request, HttpStatusCode.BadRequest);
            Equal(HttpStatusCode.BadRequest, response.StatusCode, "external contract status");
            Equal(null, response.Content, "external contract content");
        }

        private static async Task<HttpResponseMessage> SendThroughNormalizer(
            HttpRequestMessage request, HttpStatusCode statusCode)
        {
            var handler = new CorrelationIdHandler
            {
                InnerHandler = new ApiErrorNormalizationHandler
                {
                    InnerHandler = new StatusResponseHandler(statusCode)
                }
            };
            using (var invoker = new HttpMessageInvoker(handler))
                return await invoker.SendAsync(request, CancellationToken.None);
        }

        private static HttpRequestMessage NewRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/test");
            request.SetConfiguration(new HttpConfiguration());
            return request;
        }

        private static async Task<HttpResponseMessage> SendThroughCorrelationHandler(HttpRequestMessage request)
        {
            var handler = new CorrelationIdHandler { InnerHandler = new ErrorResponseHandler() };
            using (var invoker = new HttpMessageInvoker(handler))
                return await invoker.SendAsync(request, CancellationToken.None);
        }

        private static string Header(HttpResponseMessage response)
        {
            return string.Join("", response.Headers.GetValues(CorrelationIdHandler.HeaderName));
        }

        private static void Equal<T>(T expected, T actual, string name)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException(name + ": expected " + expected + ", got " + actual + ".");
        }

        private static void Present(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException(name + " is missing.");
        }

        private static void False(bool value, string message)
        {
            if (value) throw new InvalidOperationException(message);
        }

        private sealed class ErrorResponseHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(ApiErrorResponses.Create(request, HttpStatusCode.BadRequest,
                    ErrorCodes.InvalidRequest, "Invalid request."));
            }
        }

        private sealed class StatusResponseHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;

            public StatusResponseHandler(HttpStatusCode statusCode)
            {
                _statusCode = statusCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(_statusCode) { RequestMessage = request });
            }
        }
    }
}
