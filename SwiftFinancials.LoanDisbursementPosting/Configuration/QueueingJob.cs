using Application.MainBoundedContext.BackOfficeModule.Services;
using Application.MainBoundedContext.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using Quartz;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Configuration
{
    public class QueueingJob : IJob
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly ILogger _logger;

        public QueueingJob(
            IMessageQueueService messageQueueService,
            ILogger logger)
        {
            if (messageQueueService == null)
                throw new ArgumentNullException(nameof(messageQueueService));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _messageQueueService = messageQueueService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Debug("{0}****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail.Key, context.FireTimeUtc.ToString("r"), context.NextFireTimeUtc.Value.ToString("r"));

                var loanDisbursementBatchPostingConfigSection = (LoanDisbursementBatchPostingConfigSection)ConfigurationManager.GetSection("loanDisbursementBatchPostingConfiguration");

                if (loanDisbursementBatchPostingConfigSection != null)
                {
                    foreach (var settingsItem in loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems)
                    {
                        var loanDisbursementBatchPostingSettingsElement = (LoanDisbursementBatchPostingSettingsElement)settingsItem;

                        if (loanDisbursementBatchPostingSettingsElement != null && loanDisbursementBatchPostingSettingsElement.Enabled == 1)
                        {
                            var serviceHeader = new ServiceHeader { ApplicationDomainName = loanDisbursementBatchPostingSettingsElement.UniqueId };

                            var pageCollectionInfo = Container.Current.Resolve<ILoanDisbursementBatchAppService>()
                                .FindQueableLoanDisbursementBatchEntries(0, loanDisbursementBatchPostingSettingsElement.QueuePageSize, serviceHeader);

                            if (pageCollectionInfo != null && pageCollectionInfo.PageCollection != null)
                            {
                                foreach (var item in pageCollectionInfo.PageCollection)
                                {
                                    var queueDTO = new QueueDTO
                                    {
                                        RecordId = item.Id,
                                        AppDomainName = serviceHeader.ApplicationDomainName,
                                    };

                                    _messageQueueService.Send(loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueuePath, queueDTO, MessageCategory.LoanDisbursementBatchEntry, (Infrastructure.Crosscutting.Framework.Utils.MessagePriority)item.LoanDisbursementBatchPriority, loanDisbursementBatchPostingSettingsElement.TimeToBeReceived);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SwiftFinancials.LoanDisbursementBatchPosting_QueueingJob_Execute", ex);
            }
        }
    }
}
