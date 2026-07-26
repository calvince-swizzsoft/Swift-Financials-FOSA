using Application.MainBoundedContext.AdministrationModule.Services;
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AdministrationModule;
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
    public class BankService : IBankService
    {
        private readonly IBankAppService _bankAppService;

        public BankService(
            IBankAppService bankAppService)
        {
            Guard.ArgumentNotNull(bankAppService, nameof(bankAppService));

            _bankAppService = bankAppService;
        }

        #region Bank

        public BankDTO AddBank(BankDTO bankDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.AddNewBank(bankDTO, serviceHeader);
        }

        public bool UpdateBank(BankDTO bankDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.UpdateBank(bankDTO, serviceHeader);
        }

        public List<BankDTO> FindBanks()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.FindBanks(serviceHeader);
        }

        public BankDTO FindBank(Guid bankId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.FindBank(bankId, serviceHeader);
        }

        public PageCollectionInfo<BankDTO> FindBanksInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.FindBanks(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<BankDTO> FindBanksByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.FindBanks(text, pageIndex, pageSize, serviceHeader);
        }

        public List<BankBranchDTO> FindBankBranchesByBankId(Guid bankId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.FindBankBranches(bankId, serviceHeader);
        }

        public bool UpdateBankBranchesByBankId(Guid bankId, List<BankBranchDTO> nextOfKinCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.UpdateBankBranches(bankId, nextOfKinCollection, serviceHeader);
        }

        public bool BulkImport(List<string> bankCodes, List<string> bankNames, List<string> branchCodes, List<string> branchNames, List<int> branchIndexes)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _bankAppService.BulkImport(bankCodes, bankNames, branchCodes, branchNames, branchIndexes, serviceHeader);
        }

        #endregion
    }
}
