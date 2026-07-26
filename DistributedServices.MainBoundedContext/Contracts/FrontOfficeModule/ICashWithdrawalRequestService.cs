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
    public interface ICashWithdrawalRequestService
    {
        #region Cash Withdrawal Request

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CashWithdrawalRequestDTO AddCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool AuthorizeCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, int customerTransactionAuthOption);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PayCashWithdrawalRequest(CashWithdrawalRequestDTO cashWithdrawalRequestDTO, PaymentVoucherDTO paymentVoucherDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CashWithdrawalRequestDTO> FindCashWithdrawalRequests();

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByCustomerAccountId(CustomerAccountDTO customerAccountDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        List<CashWithdrawalRequestDTO> FindMatureCashWithdrawalRequestsByChartOfAccountId(Guid chartOfAccountId);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        PageCollectionInfo<CashWithdrawalRequestDTO> FindCashWithdrawalRequestsByFilterInPage(DateTime startDate, DateTime endDate, int status, string text, int customerFilter, int pageIndex, int pageSize);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        CashWithdrawalRequestDTO FindCashWithdrawalRequest(Guid cashWithdrawalRequestId);

        #endregion
    }
}
