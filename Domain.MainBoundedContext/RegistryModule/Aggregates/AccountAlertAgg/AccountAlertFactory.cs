using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.AccountAlertAgg
{
    public static class AccountAlertFactory
    {
        public static AccountAlert CreateAccountAlert(Guid customerId, int type, decimal threshold, int priority, bool maskTransactionValue, bool receiveTextAlert, bool receiveEmailAlert)
        {
            var accountAlert = new AccountAlert();

            accountAlert.GenerateNewIdentity();

            accountAlert.CustomerId = customerId;

            accountAlert.Type = (short)type;

            accountAlert.Threshold = threshold;

            accountAlert.Priority = (byte)priority;

            accountAlert.MaskTransactionValue = maskTransactionValue;

            accountAlert.ReceiveTextAlert = receiveTextAlert;

            accountAlert.ReceiveEmailAlert = receiveEmailAlert;

            accountAlert.CreatedDate = DateTime.Now;

            return accountAlert;
        }
    }
}
