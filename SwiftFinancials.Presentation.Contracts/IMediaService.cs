using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace SwiftFinancials.Presentation.Contracts
{
    [ServiceContract(Name = "IMediaService")]
    public interface IMediaService
    {
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMediaUpload(FileData fileData, AsyncCallback callback, Object state);
        string EndMediaUpload(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginMediaUploadDone(string filename, AsyncCallback callback, Object state);
        bool EndMediaUploadDone(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginGetMedia(Guid sku, AsyncCallback callback, Object state);
        MediaDTO EndGetMedia(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostFile(MediaDTO mediaDTO, AsyncCallback callback, Object state);
        bool EndPostFile(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPostImage(MediaDTO mediaDTO, AsyncCallback callback, Object state);
        bool EndPostImage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool chargeforPrinting, bool includeInterestStatement, int moduleNavigationItemCode, AsyncCallback callback, Object state);
        MediaDTO EndPrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(ApplicationServiceError))]
        IAsyncResult BeginPrintLoanRepaymentSchedule(LoanCaseDTO loanCaseDTO, AsyncCallback callback, Object state);
        MediaDTO EndPrintLoanRepaymentSchedule(IAsyncResult result);
    }
}
