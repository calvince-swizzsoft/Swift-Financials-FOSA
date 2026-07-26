using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IInHouseChequeService")]
    public interface IInHouseChequeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInHouseCheque(InHouseChequeDTO inHouseChequeDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        InHouseChequeDTO EndAddInHouseCheque(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddInHouseCheques(List<InHouseChequeDTO> inHouseChequeDTOs, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndAddInHouseCheques(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInHouseChequesInPage(int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InHouseChequeDTO> EndFindInHouseChequesInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInHouseChequesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InHouseChequeDTO> EndFindInHouseChequesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindInHouseChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InHouseChequeDTO> EndFindInHouseChequesByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnPrintedInHouseChequesByBranchIdAndFilterInPage(Guid branchId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<InHouseChequeDTO> EndFindUnPrintedInHouseChequesByBranchIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPrintInHouseCheque(InHouseChequeDTO inHouseChequeDTO, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndPrintInHouseCheque(IAsyncResult result);
    }
}
