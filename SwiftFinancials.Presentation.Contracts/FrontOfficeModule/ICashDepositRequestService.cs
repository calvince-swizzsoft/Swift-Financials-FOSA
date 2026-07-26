using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts.FrontOfficeModule
{
    [ServiceContract(Name = "ICashDepositRequestService")]
    public interface ICashDepositRequestService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAddCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, AsyncCallback callback, Object state);
        CashDepositRequestDTO EndAddCashDepositRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginAuthorizeCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, int customerTransactionAuthOption, AsyncCallback callback, Object state);
        bool EndAuthorizeCashDepositRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, AsyncCallback callback, Object state);
        bool EndPostCashDepositRequest(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashDepositRequests(AsyncCallback callback, Object state);
        List<CashDepositRequestDTO> EndFindCashDepositRequests(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindActionableCashDepositRequestsByCustomerAccount(CustomerAccountDTO customerAccountDTO, AsyncCallback callback, Object state);
        List<CashDepositRequestDTO> EndFindActionableCashDepositRequestsByCustomerAccount(IAsyncResult result);
        
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashDepositRequestsByFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize, AsyncCallback callback, Object state);
        PageCollectionInfo<CashDepositRequestDTO> EndFindCashDepositRequestsByFilterInPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginFindCashDepositRequest(Guid cashDepositRequestId, AsyncCallback callback, Object state);
        CashDepositRequestDTO EndFindCashDepositRequest(IAsyncResult result);
    }
}
