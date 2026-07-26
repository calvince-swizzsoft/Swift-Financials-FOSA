using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg
{
    public static class ChartOfAccountFactory
    {
        public static ChartOfAccount CreateChartOfAccount(ChartOfAccountType accountType, ChartOfAccountCategory accountCategory, int accountCode, string accountName, bool isControlAccount, bool isReconciliationAccount, bool postAutomaticallyOnly, Guid? costCenterId)
        {
            var chartOfAccount = new ChartOfAccount();

            chartOfAccount.GenerateNewIdentity();

            chartOfAccount.AccountType = (short)accountType;

            chartOfAccount.AccountCategory = (short)accountCategory;

            chartOfAccount.AccountCode = accountCode;

            chartOfAccount.AccountName = accountName;

            chartOfAccount.IsControlAccount = isControlAccount;

            chartOfAccount.IsReconciliationAccount = isReconciliationAccount;

            chartOfAccount.PostAutomaticallyOnly = postAutomaticallyOnly;

            chartOfAccount.CostCenterId = (!isControlAccount && costCenterId != null && costCenterId != Guid.Empty) ? costCenterId : null;

            chartOfAccount.CreatedDate = DateTime.Now;

            return chartOfAccount;
        }

        public static ChartOfAccount CreateChartOfAccount(ChartOfAccount parent, ChartOfAccountCategory accountCategory, int accountCode, string accountName, bool isControlAccount, bool isReconciliationAccount, bool postAutomaticallyOnly, Guid? costCenterId)
        {
            var chartOfAccount = new ChartOfAccount();

            chartOfAccount.GenerateNewIdentity();

            chartOfAccount.AccountType = (short)parent.AccountType;

            chartOfAccount.AccountCategory = (short)accountCategory;

            chartOfAccount.AccountCode = accountCode;

            chartOfAccount.AccountName = accountName;

            chartOfAccount.IsControlAccount = isControlAccount;

            chartOfAccount.IsReconciliationAccount = isReconciliationAccount;

            chartOfAccount.PostAutomaticallyOnly = postAutomaticallyOnly;

            chartOfAccount.SetParentChartOfAccount(parent);

            chartOfAccount.CostCenterId = (!isControlAccount && costCenterId != null && costCenterId != Guid.Empty) ? costCenterId : null;

            chartOfAccount.CreatedDate = DateTime.Now;

            return chartOfAccount;
        }
    }
}
