
using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
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
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationAppService _designationAppService;

        public DesignationService(
            IDesignationAppService designationAppService)
        {
            Guard.ArgumentNotNull(designationAppService, nameof(designationAppService));

            _designationAppService = designationAppService;
        }

        #region Designation

        public DesignationDTO AddDesignation(DesignationDTO designationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.AddNewDesignation(designationDTO, serviceHeader);
        }

        public bool UpdateDesignation(DesignationDTO designationDTO)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.UpdateDesignation(designationDTO, serviceHeader);
        }

        public List<DesignationDTO> FindDesignations()
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindDesignations(serviceHeader);
        }

        public PageCollectionInfo<DesignationDTO> FindDesignationsInPage(int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindDesignations(pageIndex, pageSize, serviceHeader);
        }

        public PageCollectionInfo<DesignationDTO> FindDesignationsByFilterInPage(string text, int pageIndex, int pageSize)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindDesignations(text, pageIndex, pageSize, serviceHeader);
        }

        public List<DesignationDTO> FindDesignationsTraverse(bool updateDepth, bool traverseTree)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindDesignations(serviceHeader, updateDepth, traverseTree);
        }

        public DesignationDTO FindDesignation(Guid designationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindDesignation(designationId, serviceHeader);
        }

        public List<TransactionThresholdDTO> FindTransactionThresholdCollectionByDesignationId(Guid designationId)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindTransactionThresholdCollection(designationId, serviceHeader);
        }

        public List<TransactionThresholdDTO> FindTransactionThresholdCollectionByDesignationIdAndTransactionThresholdType(Guid designationId, int transactionThresholdType)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.FindTransactionThresholdCollection(designationId, transactionThresholdType, serviceHeader);
        }

        public bool UpdateTransactionThresholdCollectionByDesignationId(Guid designationId, List<TransactionThresholdDTO> transactionThresholdCollection)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.UpdateTransactionThresholdCollection(designationId, transactionThresholdCollection, serviceHeader);
        }

        public bool ValidateTransactionThreshold(Guid designationId, int transactionThresholdType, decimal transactionAmount)
        {
            var serviceHeader = CustomHeaderUtility.ReadHeader(OperationContext.Current);

            return _designationAppService.ValidateTransactionThreshold(designationId, transactionThresholdType, transactionAmount, serviceHeader);
        }

        #endregion
    }
}
