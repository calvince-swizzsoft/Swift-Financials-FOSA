using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "IExternalChequeService")]
    public interface IExternalChequeService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddExternalCheque(ExternalChequeDTO externalChequeDTO, AsyncCallback callback, Object state);
        ExternalChequeDTO EndAddExternalCheque(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnClearedExternalChequesByCustomerAccountId(Guid customerAccountId, AsyncCallback callback, Object state);
        List<ExternalChequeDTO> EndFindUnClearedExternalChequesByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMarkExternalChequeCleared(Guid externalChequeId, AsyncCallback callback, Object state);
        bool EndMarkExternalChequeCleared(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnTransferredExternalChequesByTellerIdAndFilter(Guid tellerId, string text, AsyncCallback callback, Object state);
        List<ExternalChequeDTO> EndFindUnTransferredExternalChequesByTellerIdAndFilter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnTransferredExternalChequesByTellerIdAndFilterInPage(Guid tellerId, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExternalChequeDTO> EndFindUnTransferredExternalChequesByTellerIdAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExternalChequesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExternalChequeDTO> EndFindExternalChequesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExternalChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExternalChequeDTO> EndFindExternalChequesByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginTransferExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, TellerDTO tellerDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndTransferExternalCheques(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnClearedExternalChequesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExternalChequeDTO> EndFindUnClearedExternalChequesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnClearedExternalChequesByDateRangeAndFilterInPage(DateTime startDate, DateTime endDate, string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExternalChequeDTO> EndFindUnClearedExternalChequesByDateRangeAndFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginClearExternalCheque(ExternalChequeDTO externalChequeDTO, int clearingOption, int moduleNavigationItemCode, UnPayReasonDTO unPayReasonDTO, AsyncCallback callback, Object state);
        bool EndClearExternalCheque(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindUnBankedExternalChequesByFilterInPage(string text, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<ExternalChequeDTO> EndFindUnBankedExternalChequesByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginBankExternalCheques(List<ExternalChequeDTO> externalChequeDTOs, BankLinkageDTO bankLinkageDTO, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        bool EndBankExternalCheques(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindExternalChequePayablesByExternalChequeId(Guid externalChequeId, AsyncCallback callback, Object state);
        List<ExternalChequePayableDTO> EndFindExternalChequePayablesByExternalChequeId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginUpdateExternalChequePayablesByExternalChequeId(Guid externalChequeId, List<ExternalChequePayableDTO> externalChequePayables, AsyncCallback callback, Object state);
        bool EndUpdateExternalChequePayablesByExternalChequeId(IAsyncResult result);
        void BeginFindUnTransferredExternalChequesByTellerId(Guid tellerId, AsyncCallback asyncCallback, IExternalChequeService service);
    }
}
