using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Application.MainBoundedContext.Services
{
    public interface IMediaAppService
    {
        MediaDTO GetMedia(Guid sku, string blobDatabaseConnectionString);

        bool PostFile(MediaDTO mediaDTO, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader);

        bool PostImage(MediaDTO mediaDTO, string fileDirectory, string blobDatabaseConnectionString, ServiceHeader serviceHeader);

        MediaDTO PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange(CustomerAccountDTO customerAccountDTO, DateTime startDate, DateTime endDate, bool chargeForPrinting, bool includeInterestStatement, int moduleNavigationItemCode, string blobDatabaseConnectionString, ServiceHeader serviceHeader);

        MediaDTO PrintLoanRepaymentSchedule(LoanCaseDTO loanCaseDTO, string blobDatabaseConnectionString, ServiceHeader serviceHeader);
    }
}
