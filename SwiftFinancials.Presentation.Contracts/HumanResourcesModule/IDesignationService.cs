using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "IDesignationService")]
    public interface IDesignationService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddDesignation(DesignationDTO designationDTO, AsyncCallback callback, Object state);
        DesignationDTO EndAddDesignation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateDesignation(DesignationDTO designationDTO, AsyncCallback callback, Object state);
        bool EndUpdateDesignation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDesignations(AsyncCallback callback, Object state);
        List<DesignationDTO> EndFindDesignations(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDesignationsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DesignationDTO> EndFindDesignationsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDesignationsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<DesignationDTO> EndFindDesignationsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDesignationsTraverse(bool updateDepth, bool traverseTree, AsyncCallback callback, Object state);
        List<DesignationDTO> EndFindDesignationsTraverse(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindDesignation(Guid designationId, AsyncCallback callback, Object state);
        DesignationDTO EndFindDesignation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTransactionThresholdCollectionByDesignationId(Guid designationId, AsyncCallback callback, Object state);
        List<TransactionThresholdDTO> EndFindTransactionThresholdCollectionByDesignationId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindTransactionThresholdCollectionByDesignationIdAndTransactionThresholdType(Guid designationId, int transactionThresholdType, AsyncCallback callback, Object state);
        List<TransactionThresholdDTO> EndFindTransactionThresholdCollectionByDesignationIdAndTransactionThresholdType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateTransactionThresholdCollectionByDesignationId(Guid designationId, List<TransactionThresholdDTO> transactionThresholdCollection, AsyncCallback callback, Object state);
        bool EndUpdateTransactionThresholdCollectionByDesignationId(IAsyncResult result);
        
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginValidateTransactionThreshold(Guid designationId, int transactionThresholdType, decimal transactionAmount, AsyncCallback callback, Object state);
        bool EndValidateTransactionThreshold(IAsyncResult result);
    }
}
