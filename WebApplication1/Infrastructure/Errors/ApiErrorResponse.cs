using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace WebApplication1.ApiErrors
{
    public sealed class ApiErrorResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("error")]
        public ApiError Error { get; set; }

        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }
    }

    public sealed class ApiError
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("validationErrors")]
        public IDictionary<string, string[]> ValidationErrors { get; set; }
    }

    public static class ApiErrorResponses
    {
        public static HttpResponseMessage Create(HttpRequestMessage request, HttpStatusCode statusCode,
            string code, string message, IDictionary<string, string[]> validationErrors = null)
        {
            var correlationId = CorrelationIdHandler.GetCorrelationId(request);
            var payload = new ApiErrorResponse
            {
                Success = false,
                Message = message,
                Error = new ApiError { Code = code, ValidationErrors = validationErrors },
                CorrelationId = correlationId
            };

            var response = request.CreateResponse(statusCode, payload);
            CorrelationIdHandler.AddResponseHeader(response, correlationId);
            return response;
        }
    }
}
