using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.BackOfficeModule
{
    [ServiceContract(Name = "IIncomeAdjustmentService")]
    public interface IIncomeAdjustmentService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO, AsyncCallback callback, Object state);
        IncomeAdjustmentDTO EndAddIncomeAdjustment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateIncomeAdjustment(IncomeAdjustmentDTO incomeAdjustmentDTO, AsyncCallback callback, Object state);
        bool EndUpdateIncomeAdjustment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindIncomeAdjustments(AsyncCallback callback, Object state);
        List<IncomeAdjustmentDTO> EndFindIncomeAdjustments(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindIncomeAdjustment(Guid incomeAdjustmentId, AsyncCallback callback, Object state);
        IncomeAdjustmentDTO EndFindIncomeAdjustment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindIncomeAdjustmentsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<IncomeAdjustmentDTO> EndFindIncomeAdjustmentsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindIncomeAdjustmentsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<IncomeAdjustmentDTO> EndFindIncomeAdjustmentsByFilterInPage(IAsyncResult result);
    }
}
