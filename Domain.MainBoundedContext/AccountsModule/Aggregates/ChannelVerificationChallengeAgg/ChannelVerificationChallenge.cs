using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChannelVerificationChallengeAgg
{
    // A short-lived OTP challenge backing the CUSTOMER_VERIFICATION capability of the
    // SwizzChannels canonical API (POST v1/customers/verify + /verify/confirm) — proves
    // the caller controls a contact method SwiftFinancialz already has on file for the
    // (customer, account) pair before SwizzChannels links a channel identity to it.
    // Not a general-purpose OTP mechanism for anything else in this system.
    public class ChannelVerificationChallenge : Domain.Seedwork.Entity
    {
        public Guid CustomerId { get; set; }

        public Guid CustomerAccountId { get; set; }

        // SHA-256 hash of the OTP digits — never store the plaintext code.
        public string CodeHash { get; set; }

        public DateTime ExpiresAt { get; set; }

        public int Attempts { get; set; }

        public bool IsConsumed { get; private set; }

        public DateTime? ConsumedDate { get; set; }

        public void Consume()
        {
            if (!IsConsumed)
            {
                IsConsumed = true;
                ConsumedDate = DateTime.Now;
            }
        }

        public void RegisterFailedAttempt()
        {
            Attempts++;
        }
    }
}
