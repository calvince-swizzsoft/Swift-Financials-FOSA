using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.BankReconciliationEntryAgg
{
    public static class BankReconciliationEntryFactory
    {
        public static BankReconciliationEntry CreateBankReconciliationEntry(Guid bankReconciliationPeriodId, Guid? chartOfAccountId, int adjustmentType, decimal value, string chequeNumber, string chequeDrawee, DateTime? chequeDate, string remarks)
        {
            var bankReconciliationEntry = new BankReconciliationEntry();

            bankReconciliationEntry.GenerateNewIdentity();

            bankReconciliationEntry.BankReconciliationPeriodId = bankReconciliationPeriodId;

            bankReconciliationEntry.AdjustmentType = adjustmentType;

            bankReconciliationEntry.Value = value;

            // Only G/L adjustments post a double entry and therefore own a contra account.
            // Bank-side timing differences never post journals.
            if (adjustmentType == (int)BankReconciliationAdjustmentType.GeneralLedgerAccountDebit ||
                adjustmentType == (int)BankReconciliationAdjustmentType.GeneralLedgerAccountCredit)
                bankReconciliationEntry.ChartOfAccountId = chartOfAccountId != Guid.Empty ? chartOfAccountId : null;

            // Preserve optional cheque details for both bank-side and G/L adjustments.
            bankReconciliationEntry.ChequeNumber = chequeNumber;
            bankReconciliationEntry.ChequeDrawee = chequeDrawee;
            bankReconciliationEntry.ChequeDate = chequeDate;

            bankReconciliationEntry.Remarks = remarks;

            bankReconciliationEntry.CreatedDate = DateTime.Now;

            return bankReconciliationEntry;
        }
    }
}
