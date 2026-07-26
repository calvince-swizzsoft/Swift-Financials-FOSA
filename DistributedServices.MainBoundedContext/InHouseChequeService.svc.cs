using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.FrontOfficeModule.Services;
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
    public class InHouseChequeService : IInHouseChequeService
    {
        private readonly IInHouseChequeAppService _inHouseChequeAppService;

        public InHouseChequeService(
           IInHouseChequeAppService inHouseChequeAppService)
        {
            Guard.ArgumentNotNull(inHouseChequeAppService, nameof(inHouseChequeAppService));

            _inHouseChequeAppService = inHouseChequeAppService;
        }

        #region In-House Cheque

        public InHouseChequeDTO AddInHouseCheque(InHouseChequeDTO inHouseChequeDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.AddNewInHouseCheque(inHouseChequeDTO, moduleNavigationItemCode, serviceHeader);
        }

        public bool AddInHouseCheques(List<InHouseChequeDTO> inHouseChequeDTOs, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.AddNewInHouseCheques(inHouseChequeDTOs, moduleNavigationItemCode, serviceHeader);
        }

        public PageCollectionInfo<InHouseChequeDTO> FindInHouseChequesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.FindInHouseCheques(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InHouseChequeDTO> FindInHouseChequesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.FindInHouseCheques(text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InHouseChequeDTO> FindInHouseChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.FindInHouseCheques(startDate, endDate, text, pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<InHouseChequeDTO> FindUnPrintedInHouseChequesByBranchIdAndFilterInPage(Guid branchId, string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.FindUnPrintedInHouseChequesByBranchId(branchId, text, pageIndex, pageSize, serviceHeader);
        }

        public bool PrintInHouseCheque(InHouseChequeDTO inHouseChequeDTO, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _inHouseChequeAppService.PrintInHouseCheque(inHouseChequeDTO, bankLinkageDTO, moduleNavigationItemCode, serviceHeader);
        }

        #endregion
    }
}
