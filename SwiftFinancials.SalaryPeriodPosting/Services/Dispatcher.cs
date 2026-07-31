using Infrastructure.Crosscutting.Framework.Logging;
using Quartz;
using System;
using System.ComponentModel.Composition;
using System.Configuration;
using SwiftFinancials.SalaryPeriodPosting.Configuration;
using SwiftFinancials.Presentation.Infrastructure.Services;

namespace SwiftFinancials.SalaryPeriodPosting.Services
{
    [Export(typeof(IPlugin))]
    public class Dispatcher : IPlugin
    {
        private SalaryPeriodMessageProcessor _messageProcessor;

        private readonly IChannelService _channelService;

        [ImportingConstructor]
        public Dispatcher(
            IChannelService channelService)
        {
            if (channelService == null)
                throw new ArgumentNullException(nameof(channelService));

            _channelService = channelService;
        }

        #region IPlugin

        public Guid Id
        {
            get { return new Guid("{EE77B6FE-38D3-4F4F-B71E-D3095E692815}"); }
        }

        public string Description
        {
            get { return "SALARYPERIODPOSTING.DISPATCHER"; }
        }

        public void DoWork(IScheduler scheduler, params string[] args)
        {
            try
            {
                var salaryPeriodPostingConfigSection = (SalaryPeriodPostingConfigSection)ConfigurationManager.GetSection("salaryPeriodPostingConfiguration");

                if (salaryPeriodPostingConfigSection != null)
                {
                    _messageProcessor = new SalaryPeriodMessageProcessor(_channelService, salaryPeriodPostingConfigSection);

                    _messageProcessor.Open();
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLog().LogError("{0}->DoWork...", ex, Description);
            }
        }

        public void Exit()
        {
            try
            {
                if (_messageProcessor != null)
                    _messageProcessor.Close();
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLog().LogError("{0}->Exit...", ex, Description);
            }
        }

        #endregion
    }
}
