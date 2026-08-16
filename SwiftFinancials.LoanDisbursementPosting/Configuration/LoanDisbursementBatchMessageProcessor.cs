using Application.MainBoundedContext.BackOfficeModule.Services;
using Infrastructure.Crosscutting.Framework.Logging;
using Infrastructure.Crosscutting.Framework.Models;
using Infrastructure.Crosscutting.Framework.Utils;
using SwiftFinancials.AppServiceContainer;
using System;
using System.Threading.Tasks;
using Unity;

namespace SwiftFinancials.LoanDisbursementBatchPosting.Configuration
{
    public class LoanDisbursementBatchMessageProcessor : MessageProcessor<QueueDTO>
    {
        private readonly ILogger _logger;
        private readonly LoanDisbursementBatchPostingConfigSection _loanDisbursementBatchPostingConfigSection;

        public LoanDisbursementBatchMessageProcessor(ILogger logger, LoanDisbursementBatchPostingConfigSection loanDisbursementBatchPostingConfigSection)
            : base(loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueuePath, loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueueReceivers)
        {
            _logger = logger;
            _loanDisbursementBatchPostingConfigSection = loanDisbursementBatchPostingConfigSection;
        }

        protected override void LogError(Exception exception)
        {
            _logger.LogError("{0}->LoanDisbursementBatchMessageProcessor...", exception, _loanDisbursementBatchPostingConfigSection.LoanDisbursementBatchPostingSettingsItems.QueuePath);
        }

        protected override async Task Process(QueueDTO queueDTO, int appSpecific)
        {
            var serviceHeader = new ServiceHeader { ApplicationDomainName = queueDTO.AppDomainName };

            var messageCategory = (MessageCategory)appSpecific;

            switch (messageCategory)
            {
                case MessageCategory.LoanDisbursementBatchEntry:

                    Container.Current.Resolve<ILoanDisbursementBatchAppService>()
                        .PostLoanDisbursementBatchEntry(queueDTO.RecordId, 0x8888, serviceHeader);

                    break;
                default:
                    break;
            }
        }
    }
}
