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

            switch ((BankReconciliationAdjustmentType)bankReconciliationEntry.AdjustmentType)
            {
                case BankReconciliationAdjustmentType.BankAccountDebit:
                case BankReconciliationAdjustmentType.BankAccountCredit:
                    bankReconciliationEntry.ChartOfAccountId = (chartOfAccountId != null && chartOfAccountId != Guid.Empty) ? chartOfAccountId : null;
                    break;
                case BankReconciliationAdjustmentType.GeneralLedgerAccountDebit:
                case BankReconciliationAdjustmentType.GeneralLedgerAccountCredit:
                    bankReconciliationEntry.ChequeNumber = chequeNumber;
                    bankReconciliationEntry.ChequeDrawee = chequeDrawee;
                    bankReconciliationEntry.ChequeDate = chequeDate ?? null;
                    break;
                default:
                    break;
            }

            bankReconciliationEntry.Remarks = remarks;

            bankReconciliationEntry.CreatedDate = DateTime.Now;

            return bankReconciliationEntry;
        }
    }
}
