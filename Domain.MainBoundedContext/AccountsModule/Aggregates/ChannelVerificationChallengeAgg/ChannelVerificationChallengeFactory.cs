using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChannelVerificationChallengeAgg
{
    public static class ChannelVerificationChallengeFactory
    {
        public static ChannelVerificationChallenge CreateChannelVerificationChallenge(Guid customerId, Guid customerAccountId, string codeHash, DateTime expiresAt)
        {
            var challenge = new ChannelVerificationChallenge();

            challenge.GenerateNewIdentity();

            challenge.CustomerId = customerId;
            challenge.CustomerAccountId = customerAccountId;
            challenge.CodeHash = codeHash;
            challenge.ExpiresAt = expiresAt;
            challenge.Attempts = 0;
            challenge.CreatedDate = DateTime.Now;

            return challenge;
        }
    }
}
