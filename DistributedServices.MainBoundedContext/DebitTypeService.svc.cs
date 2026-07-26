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
    public class DebitTypeService : IDebitTypeService
    {
        private readonly IDebitTypeAppService _debitTypeAppService;

        public DebitTypeService(
          IDebitTypeAppService debitTypeAppService)
        {
            Guard.ArgumentNotNull(debitTypeAppService, nameof(debitTypeAppService));

            _debitTypeAppService = debitTypeAppService;
        }

        #region Debit Type

        public DebitTypeDTO AddDebitType(DebitTypeDTO debitTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitTypeAppService.AddNewDebitType(debitTypeDTO, serviceHeader);
        }

        public bool UpdateDebitType(DebitTypeDTO debitTypeDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitTypeAppService.UpdateDebitType(debitTypeDTO, serviceHeader);
        }

        public List<DebitTypeDTO> FindDebitTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitTypes = _debitTypeAppService.FindDebitTypes(serviceHeader);

            _debitTypeAppService.FetchDebitTypesProductDescription(debitTypes, serviceHeader);

            return debitTypes;
        }

        public List<DebitTypeDTO> FindMandatoryDebitTypes(bool isMandatory)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitTypeAppService.FindMandatoryDebitTypes(isMandatory, serviceHeader);             
        }

        public PageCollectionInfo<DebitTypeDTO> FindDebitTypesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitTypes = _debitTypeAppService.FindDebitTypes(pageIndex, pageSize, serviceHeader);

            if (debitTypes != null && debitTypes.PageCollection != null)
                _debitTypeAppService.FetchDebitTypesProductDescription(debitTypes.PageCollection, serviceHeader);

            return debitTypes;
        }

        public PageCollectionInfo<DebitTypeDTO> FindDebitTypesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitTypes = _debitTypeAppService.FindDebitTypes(text, pageIndex, pageSize, serviceHeader);

            if (debitTypes != null && debitTypes.PageCollection != null)
                _debitTypeAppService.FetchDebitTypesProductDescription(debitTypes.PageCollection, serviceHeader);

            return debitTypes;
        }

        public DebitTypeDTO FindDebitType(Guid debitTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            var debitType = _debitTypeAppService.FindDebitType(debitTypeId, serviceHeader);

            if (debitType != null)
                _debitTypeAppService.FetchDebitTypesProductDescription(new List<DebitTypeDTO> { debitType }, serviceHeader);

            return debitType;
        }

        public List<CommissionDTO> FindCommissionsByDebitTypeId(Guid debitTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitTypeAppService.FindCommissions(debitTypeId, serviceHeader);
        }

        public bool UpdateCommissionsByDebitTypeId(Guid debitTypeId, List<CommissionDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _debitTypeAppService.UpdateCommissions(debitTypeId, commissions, serviceHeader);
        }

        #endregion
    }
}
