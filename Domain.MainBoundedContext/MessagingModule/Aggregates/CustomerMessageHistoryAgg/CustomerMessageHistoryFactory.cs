using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg
{
    public static class CustomerMessageHistoryFactory
    {
        public static CustomerMessageHistory CreateCustomerMessageHistory(Guid customerId, string body, string subject, int messageCategory, string recipient)
        {
            var customerMessageHistory = new CustomerMessageHistory
            {
                CustomerId = customerId,
                Body = body,
                Subject = subject,
                MessageCategory = messageCategory,
                Recipient = recipient
            };

            customerMessageHistory.GenerateNewIdentity();

            customerMessageHistory.CreatedDate = DateTime.Now;

            return customerMessageHistory;
        }
    }
}
