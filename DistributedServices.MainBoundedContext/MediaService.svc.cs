using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Application.MainBoundedContext.Services;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandler()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MediaService : IMediaService
    {
        private readonly IMediaAppService _mediaAppService;

        private static Dictionary<string, FileStream> _wcfFileStreams = new Dictionary<string, FileStream>();

        private static string _lastUpdatedFilename;

        public MediaService(
            IMediaAppService mediaAppService)
        {
            Guard.ArgumentNotNull(mediaAppService, nameof(mediaAppService));

            _mediaAppService = mediaAppService;
        }

        public string MediaUpload(FileData fileData)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            FileStream fileStream;
            
            string filepath = Path.Combine(serviceBrokerSettingsElement.FileUploadDirectory, fileData.Filename);

            try
            {
                lock (_wcfFileStreams)
                {
                    _wcfFileStreams.TryGetValue(filepath, out fileStream);

                    if (fileStream == null)
                    {
                        fileStream = File.Open(filepath, FileMode.Create, FileAccess.ReadWrite);

                        _wcfFileStreams.Add(filepath, fileStream);
                    }
                }

                fileStream.Write(fileData.Buffer, 0, fileData.Buffer.Length);

                fileStream.Flush();

                return string.Empty;
            }
            catch (Exception ex)
            {
                MediaUploadDone(filepath);

                return ex.ToString();
            }
        }

        public bool MediaUploadDone(string filename)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            FileStream fileStream;

            string filepath = Path.Combine(serviceBrokerSettingsElement.FileUploadDirectory, filename);

            lock (_wcfFileStreams)
            {
                _wcfFileStreams.TryGetValue(filepath, out fileStream);

                if (fileStream != null)
                {
                    _wcfFileStreams.Remove(filepath);

                    _lastUpdatedFilename = filepath;

                    fileStream.Close();

                    return true;
                }
            }

            return false;
        }

        public MediaDTO GetMedia(Guid sku)
        {
            return _mediaAppService.GetMedia(sku, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString);
        }

        public bool PostFile(MediaDTO mediaDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _mediaAppService.PostFile(mediaDTO, serviceBrokerSettingsElement.FileUploadDirectory, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString, serviceHeader);
        }

        public bool PostImage(MediaDTO mediaDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var serviceBrokerSettingsElement = ConfigurationHelper.GetServiceBrokerConfigurationSettings(serviceHeader);

            return _mediaAppService.PostImage(mediaDTO, serviceBrokerSettingsElement.FileUploadDirectory, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString, serviceHeader);
        }

        public MediaDTO PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool chargeforPrinting, bool includeInterestStatement, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mediaAppService.PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(customerAccountDTO, startDate, endDate, chargeforPrinting, includeInterestStatement, moduleNavigationItemCode, ConfigurationManager.ConnectionStrings["BLOBStore"].ConnectionString, serviceHeader);
        }

        public MediaDTO PrintLoanRepaymentSchedule(LoanCaseDTO loanCaseDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _mediaAppService.PrintLoanRepaymentSchedule(loanCaseDTO, ConfigurationManager.ConnectionStrings["SwiftFin_Dev"].ConnectionString, serviceHeader);
        }
    }
}
