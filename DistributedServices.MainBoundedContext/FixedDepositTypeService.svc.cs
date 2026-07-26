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
    public class FixedDepositTypeService : IFixedDepositTypeService
    {
        private readonly IFixedDepositTypeAppService _fixedDepositTypeAppService;

        public FixedDepositTypeService(
           IFixedDepositTypeAppService fixedDepositTypeAppService)
        {
            Guard.ArgumentNotNull(fixedDepositTypeAppService, nameof(fixedDepositTypeAppService));

            _fixedDepositTypeAppService = fixedDepositTypeAppService;
        }

        #region Fixed Deposit Type

        public FixedDepositTypeDTO AddFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.AddNewFixedDepositType(fixedDepositTypeDTO, enforceFixedDepositBands, serviceHeader);
        }

        public bool UpdateFixedDepositType(FixedDepositTypeDTO fixedDepositTypeDTO, bool enforceFixedDepositBands)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.UpdateFixedDepositType(fixedDepositTypeDTO, enforceFixedDepositBands, serviceHeader);
        }

        public List<FixedDepositTypeDTO> FindFixedDepositTypes()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindFixedDepositTypes(serviceHeader);
        }

        public FixedDepositTypeDTO FindFixedDepositType(Guid fixedDepositTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindFixedDepositType(fixedDepositTypeId, serviceHeader);
        }

        public List<FixedDepositTypeDTO> FindFixedDepositTypesByMonths(int months)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindFixedDepositTypesByMonths(months, serviceHeader);
        }

        public PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypesInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindFixedDepositTypes(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<FixedDepositTypeDTO> FindFixedDepositTypesByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindFixedDepositTypes(text, pageIndex, pageSize, serviceHeader);
        }

        public List<LevyDTO> FindLeviesByFixedDepositTypeId(Guid fixedDepositTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindLevies(fixedDepositTypeId, serviceHeader);
        }

        public bool UpdateLeviesByFixedDepositTypeId(Guid fixedDepositTypeId, List<LevyDTO> commissions)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.UpdateLevies(fixedDepositTypeId, commissions, serviceHeader);
        }

        public ProductCollectionInfo FindAttachedProductsByFixedDepositTypeId(Guid fixedDepositTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindAttachedProducts(fixedDepositTypeId, serviceHeader);
        }

        public bool UpdateAttachedProductsByFixedDepositTypeId(Guid fixedDepositTypeId, ProductCollectionInfo attachedProductsTuple)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.UpdateAttachedProducts(fixedDepositTypeId, attachedProductsTuple, serviceHeader);
        }

        #endregion

        #region Fixed Deposit Type Graduated Scale

        public List<FixedDepositTypeGraduatedScaleDTO> FindGraduatedScalesByFixedDepositTypeId(Guid fixedDepositTypeId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.FindGraduatedScales(fixedDepositTypeId, serviceHeader);
        }

        public bool UpdateGraduatedScalesByFixedDepositTypeId(Guid fixedDepositTypeId, List<FixedDepositTypeGraduatedScaleDTO> graduatedScales)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _fixedDepositTypeAppService.UpdateGraduatedScales(fixedDepositTypeId, graduatedScales, serviceHeader);
        }

        #endregion
    }
}
