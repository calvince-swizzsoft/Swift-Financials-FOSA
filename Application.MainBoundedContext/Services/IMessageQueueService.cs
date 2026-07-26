using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.Services
{
    public interface IMessageQueueService
    {
        void Send(string queuePath, object data, MessageCategory messageCategory, MessagePriority priority, int timeToBeReceived);
    }
}
