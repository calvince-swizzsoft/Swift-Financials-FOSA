using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.HumanResourcesModule
{
    [ServiceContract(Name = "ISalaryCardService")]
    public interface ISalaryCardService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddSalaryCard(SalaryCardDTO salaryCardDTO, AsyncCallback callback, Object state);
        SalaryCardDTO EndAddSalaryCard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalaryCard(SalaryCardDTO salaryCardDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalaryCard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginResetSalaryCardEntries(SalaryCardDTO salaryCardDTO, AsyncCallback callback, Object state);
        bool EndResetSalaryCardEntries(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateSalaryCardEntry(SalaryCardEntryDTO salaryCardEntryDTO, AsyncCallback callback, Object state);
        bool EndUpdateSalaryCardEntry(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryCards(AsyncCallback callback, Object state);
        List<SalaryCardDTO> EndFindSalaryCards(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryCardsInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SalaryCardDTO> EndFindSalaryCardsInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryCardsByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<SalaryCardDTO> EndFindSalaryCardsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryCard(Guid salaryCardId, AsyncCallback callback, Object state);
        SalaryCardDTO EndFindSalaryCard(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindSalaryCardEntriesBySalaryCardId(Guid salaryCardId, AsyncCallback callback, Object state);
        List<SalaryCardEntryDTO> EndFindSalaryCardEntriesBySalaryCardId(IAsyncResult result);
    }
}
