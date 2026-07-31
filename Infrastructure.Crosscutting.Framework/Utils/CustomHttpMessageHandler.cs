using Infrastructure.Crosscutting.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public abstract class CustomHttpMessageHandler : DelegatingHandler
    {
        private const string CorrelationIdHeaderName = "X-Correlation-Id";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corrId = string.Format("{0}{1}", DateTime.Now.Ticks, Thread.CurrentThread.ManagedThreadId);

            // Log the request information
            request.Headers.Add(CorrelationIdHeaderName, corrId);
            await LogRequestLoggingInfo(corrId, request).ContinueWith(async task =>
            {
                await ProcessMessageAsync(task.Result);
            });

            // Execute the request
            var response = await base.SendAsync(request, cancellationToken);

            // Extract the response logging info then persist the information
            response.Headers.Add(CorrelationIdHeaderName, corrId);
            await LogResponseLoggingInfo(corrId, response).ContinueWith(async task =>
            {
                await ProcessMessageAsync(task.Result);
            });

            return response;
        }

        protected abstract Task ProcessMessageAsync(ApiLoggingInfo apiLoggingInfo);

        private async Task<ApiLoggingInfo> LogRequestLoggingInfo(string correlationId, HttpRequestMessage request)
        {
            var info = new ApiLoggingInfo();
            info.HttpMethod = request.Method.Method;
            info.UriAccessed = request.RequestUri.AbsoluteUri;
            info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
            info.MessageType = HttpMessageType.Request;

            ExtractMessageHeadersIntoLoggingInfo(info, request.Headers.ToList());

            if (request.Content != null)
            {
                var buffer = await request.Content.ReadAsByteArrayAsync();

                info.BodyContent = Encoding.UTF8.GetString(buffer);
            }

            return info;
        }

        private async Task<ApiLoggingInfo> LogResponseLoggingInfo(string correlationId, HttpResponseMessage response)
        {
            var info = new ApiLoggingInfo();
            info.MessageType = HttpMessageType.Response;
            info.HttpMethod = response.RequestMessage.Method.ToString();
            info.ResponseStatusCode = response.StatusCode;
            info.ResponseStatusMessage = response.ReasonPhrase;
            info.UriAccessed = response.RequestMessage.RequestUri.AbsoluteUri;
            info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";

            ExtractMessageHeadersIntoLoggingInfo(info, response.Headers.ToList());

            if (response.Content != null)
            {
                var buffer = await response.Content.ReadAsByteArrayAsync();

                info.BodyContent = Encoding.UTF8.GetString(buffer);
            }

            return info;
        }

        private void ExtractMessageHeadersIntoLoggingInfo(ApiLoggingInfo info, List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            headers.ForEach(h =>
            {
                var headerValues = new StringBuilder();

                if (h.Value != null)
                {
                    foreach (var hv in h.Value)
                    {
                        if (headerValues.Length > 0)
                        {
                            headerValues.Append(", ");
                        }

                        headerValues.Append(hv);
                    }
                }

                info.Headers.Add(string.Format("{0}: {1}", h.Key, headerValues.ToString()));
            });
        }
    }
}
