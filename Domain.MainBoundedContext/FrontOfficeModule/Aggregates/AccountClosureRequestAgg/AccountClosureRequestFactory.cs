using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.AccountClosureRequestAgg
{
    public static class AccountClosureRequestFactory
    {
        public static AccountClosureRequest CreateAccountClosureRequest(Guid customerAccountId, Guid branchId, string reason, ServiceHeader serviceHeader)
        {
            var accountClosureRequest = new AccountClosureRequest();

            accountClosureRequest.GenerateNewIdentity();

            accountClosureRequest.CustomerAccountId = customerAccountId;

            accountClosureRequest.BranchId = branchId;

            accountClosureRequest.Reason = reason;

            accountClosureRequest.CreatedBy = serviceHeader.ApplicationUserName;

            accountClosureRequest.CreatedDate = DateTime.Now;

            return accountClosureRequest;
        }
    }
}
