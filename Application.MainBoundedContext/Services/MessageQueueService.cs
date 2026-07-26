//using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.Composition;
using System.Messaging;

namespace Application.MainBoundedContext.Services
{
    [Export(typeof(IMessageQueueService))]
    public class MessageQueueService : IMessageQueueService
    {
        public void Send(string queuePath, object data, Infrastructure.Crosscutting.Framework.Utils.MessageCategory messageCategory, Infrastructure.Crosscutting.Framework.Utils.MessagePriority priority, int timeToBeReceived)
        {
            if (string.IsNullOrWhiteSpace(queuePath))
                throw new ArgumentNullException(nameof(queuePath));

            if (!MessageQueue.Exists(queuePath))
                MessageQueue.Create(queuePath, true);

            using (MessageQueue messageQueue = new MessageQueue(queuePath, QueueAccessMode.Send))
            {
                messageQueue.Formatter = new BinaryMessageFormatter();

                messageQueue.MessageReadPropertyFilter.SetAll();

                using (MessageQueueTransaction mqt = new MessageQueueTransaction())
                {
                    mqt.Begin();

                    using (Message message = new Message(data, new BinaryMessageFormatter()))
                    {
                        message.Label = string.Format("{0}|{1}", Infrastructure.Crosscutting.Framework.Utils.EnumHelper.GetDescription(messageCategory), Infrastructure.Crosscutting.Framework.Utils.EnumHelper.GetDescription(priority));
                        message.AppSpecific = (int)messageCategory;
                        message.Priority = MapPriority(priority);
                        message.TimeToBeReceived = TimeSpan.FromMinutes(timeToBeReceived);

                        messageQueue.Send(message, mqt);
                    }

                    mqt.Commit();
                }
            }
        }


        private System.Messaging.MessagePriority MapPriority(Infrastructure.Crosscutting.Framework.Utils.MessagePriority p)
        {
            switch (p)
            {
                case Infrastructure.Crosscutting.Framework.Utils.MessagePriority.Lowest: return System.Messaging.MessagePriority.Lowest;
                case Infrastructure.Crosscutting.Framework.Utils.MessagePriority.Low: return System.Messaging.MessagePriority.Low;
                case Infrastructure.Crosscutting.Framework.Utils.MessagePriority.Normal: return System.Messaging.MessagePriority.Normal;
                case Infrastructure.Crosscutting.Framework.Utils.MessagePriority.AboveNormal: return System.Messaging.MessagePriority.AboveNormal;
                case Infrastructure.Crosscutting.Framework.Utils.MessagePriority.High: return System.Messaging.MessagePriority.High;
                case Infrastructure.Crosscutting.Framework.Utils. MessagePriority.Highest: return System.Messaging.MessagePriority.Highest;
                default: return System.Messaging.MessagePriority.Normal;
            }
        }
    }
}
