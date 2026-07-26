using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertCommissionAgg
{
    public static class TextAlertCommissionFactory
    {
        public static TextAlertCommission CreateTextAlertCommission(int systemTransactionCode, Guid commissionId, int chargeBenefactor)
        {
            var textAlertCommission = new TextAlertCommission();

            textAlertCommission.GenerateNewIdentity();

            textAlertCommission.SystemTransactionCode = systemTransactionCode;

            textAlertCommission.CommissionId = commissionId;

            textAlertCommission.ChargeBenefactor = (byte)chargeBenefactor;

            textAlertCommission.CreatedDate = DateTime.Now;

            return textAlertCommission;
        }
    }
}
