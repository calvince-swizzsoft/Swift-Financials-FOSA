using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.TextAlertDispatcher.Celcom.Configuration
{
    public class TextMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly IChannelService _channelService;
        private readonly ILogger _logger;
        private readonly TextDispatcherConfigSection _textDispatcherConfigSection;

        private readonly LimitedPool<HttpClient> _httpClientPool;

        public TextMessageProcessor(IChannelService channelService, ILogger logger, TextDispatcherConfigSection textDispatcherConfigSection)
            : base(textDispatcherConfigSection.TextDispatcherSettingsItems.QueuePath, textDispatcherConfigSection.TextDispatcherSettingsItems.QueueReceivers)
        {
            _channelService = channelService;
            _logger = logger;
            _textDispatcherConfigSection = textDispatcherConfigSection;
            _httpClientPool = new LimitedPool<HttpClient>(() =>
            {
                return _textDispatcherConfigSection.TextDispatcherSettingsItems.LogEnabled == 1
                ? new HttpClient(new TextMessageLoggingHandler(_logger))
                : new HttpClient();
            }, client => client.Dispose());
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->TextMessageProcessor...", exception, _textDispatcherConfigSection.TextDispatcherSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            foreach (var settingsItem in _textDispatcherConfigSection.TextDispatcherSettingsItems)
            {
                var textDispatcherSettingsElement = (TextDispatcherSettingsElement)settingsItem;

                if (textDispatcherSettingsElement.UniqueId == queueDTO.AppDomainName)
                {
                    queueDTO.BulkTextUrl = textDispatcherSettingsElement.BulkTextUrl;
                   
                    queueDTO.BulkTextSenderId = textDispatcherSettingsElement.BulkTextSenderId;

                    var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

                    var messageCategory = (MessageCategory)appSpecific;

                    switch (messageCategory)
                    {
                        case MessageCategory.SMSAlert:

                            #region sms

                            var smsAlert = await _channelService.FindTextAlertAsync(queueDTO.RecordId, serviceHeader);

                            if (smsAlert == null) return;

                            switch ((DLRStatus)smsAlert.TextMessageDLRStatus)
                            {
                                case DLRStatus.UnKnown:
                                case DLRStatus.Pending:

                                    if (string.IsNullOrWhiteSpace(smsAlert.TextMessageRecipient) || string.IsNullOrWhiteSpace(smsAlert.TextMessageBody) || smsAlert.TextMessageSendRetry != 0) return;

                                    var msisdn = smsAlert.TextMessageRecipient.Trim();

                                    var textMessage = smsAlert.TextMessageBody.Trim();

                                    if (!Regex.IsMatch(msisdn, @"^\+(?:[0-9]??){6,14}[0-9]$")) return;

                                    var request = new request
                                    {
                                        PhoneNumber = msisdn.Replace("+", string.Empty),
                                        OrgCode = queueDTO.BulkTextSenderId,
                                        message = textMessage

                                    };

                                    var serializer = new JavaScriptSerializer();

                                    var payload = serializer.Serialize(request);

                                    var responseTuple = await PostAsync(payload, queueDTO.BulkTextUrl);

                                    if (responseTuple.Item1.In(HttpStatusCode.OK))
                                    {
                                        string[] response = responseTuple.Item2.Split(':');

                                        string[] responseCode = response[2].Split(',');
                                        string code = responseCode[0].Trim();

                                        string[] responseMessageId = response[6].Split(',');
                                        string messageid = responseMessageId[0].Trim();

                                        switch (code)
                                        {
                                            case "200":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Delivered;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Successful Request Call", messageid);
                                                break;
                                            case "1001":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Invalid sender id", messageid);
                                                break;
                                            case "1002":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Network not allowed", messageid);
                                                break;
                                            case "1003":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Invalid mobile number", messageid);
                                                break;
                                            case "1004":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Low bulk credits", messageid);
                                                break;
                                            case "1005":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Failed.System error", messageid);
                                                break;
                                            case "1006":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Invalid credentials", messageid);
                                                break;
                                            case "1007":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Failed.System error", messageid);
                                                break;
                                            case "1008":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|No Delivery Report", messageid);
                                                break;
                                            case "1009":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|unsupported data type", messageid);
                                                break;
                                            case "1010":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|unsupported request type", messageid);
                                                break;
                                            case "4090":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Internal Error. Try again after 5 minutes", messageid);
                                                break;
                                            case "4091":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|No Partner ID is Set", messageid);
                                                break;
                                            case "4092":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|No API KEY Provided", messageid);
                                                break;
                                            case "4093":
                                                smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                                smsAlert.TextMessageSendRetry += 1;
                                                smsAlert.TextMessageReference = string.Format("{0}|Details Not Found", messageid);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        smsAlert.TextMessageDLRStatus = (int)DLRStatus.Failed;
                                        smsAlert.TextMessageSendRetry += 1;
                                        smsAlert.TextMessageReference = string.Format("{0}:{1}", responseTuple.Item1, responseTuple.Item2);
                                    }

                                    await _channelService.UpdateTextAlertAsync(smsAlert, serviceHeader);

                                    break;
                                default:
                                    break;
                            }

                            #endregion

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private async Task<Tuple<HttpStatusCode, string>> PostAsync(string payload, string requestUriString)
        {
            using (var httpClientContainer = _httpClientPool.Get())
            {
                var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await httpClientContainer.Value.PostAsync(requestUriString, httpContent);

                var responseMessage = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;

                return new Tuple<HttpStatusCode, string>(response.StatusCode, responseMessage);
            }
        }
    }

    public class request
    {
        public string apikey { get; set; }
        public string partnerID { get; set; }
        public string message { get; set; }
        public string shortcode { get; set; }
        public string mobile { get; set; }
        public string timeToSend { get; set; }
        public string OrgCode { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class response
    {
        public List<result> results { get; set; }
    }

    public class authentication
    {
        public string username { get; set; }

        public string password { get; set; }
    }

    public class message
    {
        public string sender { get; set; }

        public string text { get; set; }

        public List<recipient> recipients { get; set; }
    }

    public class recipient
    {
        public string gsm { get; set; }
    }

    public class result
    {
        public string code { get; set; }
        public string description { get; set; }
        public string mobile { get; set; }
        public string messageid { get; set; }
        public string networkid { get; set; }
    }
}
