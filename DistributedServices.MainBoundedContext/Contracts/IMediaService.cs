using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using DistributedServices.Seedwork.ErrorHandlers;
using System;
using System.ServiceModel;

namespace DistributedServices.MainBoundedContext
{
    [ServiceContract]
    public interface IMediaService
    {
        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        string MediaUpload(FileData fileData);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        bool MediaUploadDone(string filename);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        MediaDTO GetMedia(Guid sku);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostFile(MediaDTO mediaDTO);

        [OperationContract]
        [FaultContract(typeof(ApplicationServiceError))]
        bool PostImage(MediaDTO mediaDTO);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MediaDTO PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool chargeforPrinting, bool includeInterestStatement, int moduleNavigationItemCode);

        [OperationContract()]
        [FaultContract(typeof(ApplicationServiceError))]
        MediaDTO PrintLoanRepaymentSchedule(LoanCaseDTO loanCaseDTO);
    }
}
