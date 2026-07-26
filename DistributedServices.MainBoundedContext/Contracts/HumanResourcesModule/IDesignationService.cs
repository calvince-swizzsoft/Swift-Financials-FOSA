using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IDesignationService
    {
        #region Designation

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DesignationDTO AddDesignation(DesignationDTO designationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateDesignation(DesignationDTO designationDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DesignationDTO> FindDesignations();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DesignationDTO> FindDesignationsInPage(int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<DesignationDTO> FindDesignationsByFilterInPage(string text, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<DesignationDTO> FindDesignationsTraverse(bool updateDepth, bool traverseTree);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        DesignationDTO FindDesignation(Guid designationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TransactionThresholdDTO> FindTransactionThresholdCollectionByDesignationId(Guid designationId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<TransactionThresholdDTO> FindTransactionThresholdCollectionByDesignationIdAndTransactionThresholdType(Guid designationId, int transactionThresholdType);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool UpdateTransactionThresholdCollectionByDesignationId(Guid designationId, List<TransactionThresholdDTO> transactionThresholdCollection);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool ValidateTransactionThreshold(Guid designationId, int transactionThresholdType, decimal transactionAmount);

        #endregion
    }
}
