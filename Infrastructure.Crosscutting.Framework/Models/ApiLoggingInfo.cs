using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Infrastructure.Crosscutting.Framework.Models
{
    public class ApiLoggingInfo
    {
        private List<string> _headers = new List<string>();

        public string HttpMethod { get; set; }
        public string UriAccessed { get; set; }
        public string BodyContent { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseStatusMessage { get; set; }
        public string IpAddress { get; set; }
        public HttpMessageType MessageType { get; set; }

        public List<string> Headers
        {
            get { return _headers; }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Format("MessageType: {0}", MessageType));
            if (MessageType == HttpMessageType.Response)
                stringBuilder.AppendLine(string.Format("ResponseStatusCode: {0} {1}", (int)ResponseStatusCode, ResponseStatusMessage));
            stringBuilder.AppendLine(string.Format("UriAccessed: {0}", UriAccessed));
            stringBuilder.AppendLine(string.Format("HttpMethod: {0}", HttpMethod));
            stringBuilder.AppendLine(string.Format("IpAddress: {0}", IpAddress));

            stringBuilder.AppendLine(string.Join(Environment.NewLine, Headers));

            if (!string.IsNullOrWhiteSpace(BodyContent))
                stringBuilder.AppendLine(BodyContent);

            return string.Format("{0}", stringBuilder);
        }
    }
}
