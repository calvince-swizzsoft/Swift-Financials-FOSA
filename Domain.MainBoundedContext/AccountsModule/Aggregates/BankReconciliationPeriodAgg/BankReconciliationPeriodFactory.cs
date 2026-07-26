using Domain.MainBoundedContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationPeriodAgg
{
    public static class BankReconciliationPeriodFactory
    {
        public static BankReconciliationPeriod CreateBankReconciliationPeriod(Guid branchId, Guid postingPeriodId, Guid bankLinkageId, Guid chartOfAccountId, string bankAccountNumber, Duration duration, decimal bankAccountBalance, decimal generalLedgerAccountBalance, string remarks)
        {
            var bankReconciliationPeriod = new BankReconciliationPeriod();

            bankReconciliationPeriod.GenerateNewIdentity();

            bankReconciliationPeriod.BranchId = branchId;

            bankReconciliationPeriod.PostingPeriodId = postingPeriodId;

            bankReconciliationPeriod.BankLinkageId = bankLinkageId;

            bankReconciliationPeriod.ChartOfAccountId = chartOfAccountId;

            bankReconciliationPeriod.BankAccountNumber = bankAccountNumber;

            bankReconciliationPeriod.Duration = duration;

            bankReconciliationPeriod.BankAccountBalance = bankAccountBalance;

            bankReconciliationPeriod.GeneralLedgerAccountBalance = generalLedgerAccountBalance;

            bankReconciliationPeriod.Remarks = remarks;

            bankReconciliationPeriod.CreatedDate = DateTime.Now;

            return bankReconciliationPeriod;
        }
    }
}
