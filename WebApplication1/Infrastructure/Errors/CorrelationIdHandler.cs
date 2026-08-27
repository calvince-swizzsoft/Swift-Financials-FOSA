using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.ApiErrors
{
    public sealed class CorrelationIdHandler : DelegatingHandler
    {
        public const string HeaderName = "X-Correlation-ID";
        private const string RequestPropertyName = "SwiftFinancials.CorrelationId";
        private static readonly Regex ValidCorrelationId =
            new Regex("^[A-Za-z0-9][A-Za-z0-9._-]{0,63}$", RegexOptions.Compiled);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = ReadIncomingCorrelationId(request) ?? Guid.NewGuid().ToString("N");
            request.Properties[RequestPropertyName] = correlationId;

            var response = await base.SendAsync(request, cancellationToken);
            AddResponseHeader(response, correlationId);
            return response;
        }

        public static string GetCorrelationId(HttpRequestMessage request)
        {
            object value;
            if (request != null && request.Properties.TryGetValue(RequestPropertyName, out value))
                return value as string ?? Guid.NewGuid().ToString("N");
            return Guid.NewGuid().ToString("N");
        }

        public static void AddResponseHeader(HttpResponseMessage response, string correlationId)
        {
            if (response == null || string.IsNullOrWhiteSpace(correlationId)) return;
            response.Headers.Remove(HeaderName);
            response.Headers.TryAddWithoutValidation(HeaderName, correlationId);
        }

        private static string ReadIncomingCorrelationId(HttpRequestMessage request)
        {
            System.Collections.Generic.IEnumerable<string> values;
            if (!request.Headers.TryGetValues(HeaderName, out values)) return null;
            var value = values.FirstOrDefault();
            return !string.IsNullOrWhiteSpace(value) && ValidCorrelationId.IsMatch(value) ? value : null;
        }
    }
}
