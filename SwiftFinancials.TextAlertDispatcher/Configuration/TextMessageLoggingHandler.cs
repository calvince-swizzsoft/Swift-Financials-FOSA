using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Configuration
{
    class TextMessageLoggingHandler : CustomHttpMessageHandler
    {
        private readonly ILogger _logger;

        public TextMessageLoggingHandler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InnerHandler = new HttpClientHandler();
        }

        protected override Task ProcessMessageAsync(ApiLoggingInfo apiLoggingInfo)
        {
            return Task.Run(() =>
            {
                _logger.LogInfo("TEXTDISPATCHER>TextMessageLoggingHandler\r\n{0}", apiLoggingInfo);
            });
        }
    }
}
