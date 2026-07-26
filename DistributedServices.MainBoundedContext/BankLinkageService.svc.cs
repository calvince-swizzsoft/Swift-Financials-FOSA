using Application.MainBoundedContext.AccountsModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using DistributedServices.MainBoundedContext.InstanceProviders;
using DistributedServices.Seedwork.EndpointBehaviors;
using DistributedServices.Seedwork.ErrorHandlers;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ApplicationErrorHandlerAttribute()]
    [UnityInstanceProviderServiceBehavior()]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BankLinkageService : IBankLinkageService
    {
        private readonly IBankLinkageAppService _bankLinkageAppService;

        public BankLinkageService(
            IBankLinkageAppService bankLinkageAppService)
        {
            Guard.ArgumentNotNull(bankLinkageAppService, nameof(bankLinkageAppService));

            _bankLinkageAppService = bankLinkageAppService;
        }

        #region Bank Linkage

        public BankLinkageDTO AddBankLinkage(BankLinkageDTO bankLinkageDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.AddNewBankLinkage(bankLinkageDTO, serviceHeader);
        }

        public bool UpdateBankLinkage(BankLinkageDTO bankLinkageDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.UpdateBankLinkage(bankLinkageDTO, serviceHeader);
        }

        public List<BankLinkageDTO> FindBankLinkages()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.FindBankLinkages(serviceHeader);
        }

        public PageCollectionInfo<BankLinkageDTO> FindBankLinkagesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.FindBankLinkages(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<BankLinkageDTO> FindBankLinkagesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.FindBankLinkages(text, pageIndex, pageSize, serviceHeader);
        }

        public BankLinkageDTO FindBankLinkage(Guid bankLinkageId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.FindBankLinkage(bankLinkageId, serviceHeader);
        }

        public BankLinkageDTO FindBankLinkageByBankAccountId(Guid bankAccountId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankLinkageAppService.FindBankLinkageByBankAccountId(bankAccountId, serviceHeader);
        }

        #endregion
    }
}
