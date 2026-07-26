using Domain.Seedwork;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.MessageGroupAgg
{
    public class MessageGroup : Entity
    {
        public string Description { get; set; }
        
        public byte Target { get; set; }

        public string TargetValues { get; set; }
    }
}
