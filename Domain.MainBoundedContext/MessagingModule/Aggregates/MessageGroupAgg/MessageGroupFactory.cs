using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.MessageGroupAgg
{
    public static class MessageGroupFactory
    {
        public static MessageGroup CreateMessageGroup(string description, int target, string targetValues)
        {
            var messageGroup = new MessageGroup
            {
                Description = description,
                Target = (byte)target,
                TargetValues = targetValues,
            };

            messageGroup.GenerateNewIdentity();

            messageGroup.CreatedDate = DateTime.Now;

            return messageGroup;
        }
    }
}
