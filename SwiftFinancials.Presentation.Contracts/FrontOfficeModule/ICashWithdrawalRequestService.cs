using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "ICashWithdrawalRequestService")]
    public interface ICashWithdrawalRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, AsyncCallback callback, Object state);
        CashWithdrawalRequestDTO EndAddCashWithdrawalRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, int customerTransactionAuthOption, AsyncCallback callback, Object state);
        bool EndAuthorizeCashWithdrawalRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPayCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, PaymentVoucherDTO paymentVoucherDTO, AsyncCallback callback, Object state);
        bool EndPayCashWithdrawalRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashWithdrawalRequests(AsyncCallback callback, Object state);
        List<CashWithdrawalRequestDTO> EndFindCashWithdrawalRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMatureCashWithdrawalRequestsByCustomerAccountId(CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<CashWithdrawalRequestDTO> EndFindMatureCashWithdrawalRequestsByCustomerAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindMatureCashWithdrawalRequestsByChartOfAccountId(Guid chartOfAccountId, AsyncCallback callback, Object state);
        List<CashWithdrawalRequestDTO> EndFindMatureCashWithdrawalRequestsByChartOfAccountId(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashWithdrawalRequestsByFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CashWithdrawalRequestDTO> EndFindCashWithdrawalRequestsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashWithdrawalRequest(Guid cashWithdrawalRequestId, AsyncCallback callback, Object state);
        CashWithdrawalRequestDTO EndFindCashWithdrawalRequest(IAsyncResult result);
    }
}
