using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelAgg
{
    public static class AlternateChannelFactory
    {
        public static AlternateChannel CreateAlternateChannel(Guid customerAccountId, int type, string cardNumber, DateTime validFrom, DateTime expires, decimal dailyLimit, string recruitedBy)
        {
            var alternateChannel = new AlternateChannel();

            alternateChannel.GenerateNewIdentity();

            alternateChannel.CustomerAccountId = customerAccountId;

            alternateChannel.Type = (short)type;

            alternateChannel.CardNumber = (cardNumber ?? string.Empty).Trim();

            alternateChannel.ValidFrom = validFrom;

            alternateChannel.Expires = expires;

            alternateChannel.DailyLimit = dailyLimit;

            alternateChannel.RecruitedBy = recruitedBy;

            alternateChannel.CreatedDate = DateTime.Now;

            return alternateChannel;
        }
    }
}
