using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface ICashDepositRequestService
    {
        #region Cash Deposit Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CashDepositRequestDTO AddCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO, int customerTransactionAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostCashDepositRequest(CashDepositRequestDTO cashDepositRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CashDepositRequestDTO> FindCashDepositRequests();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CashDepositRequestDTO> FindActionableCashDepositRequestsByCustomerAccount(CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CashDepositRequestDTO> FindCashDepositRequestsByFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CashDepositRequestDTO FindCashDepositRequest(Guid cashDepositRequestId);

        #endregion
    }
}
