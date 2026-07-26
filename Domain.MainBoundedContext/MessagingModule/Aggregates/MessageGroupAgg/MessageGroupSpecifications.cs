using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.MessageGroupAgg
{
    public static class MessageGroupSpecifications
    {
        public static Specification<MessageGroup> DefaultSpec()
        {
            Specification<MessageGroup> specification = new TrueSpecification<MessageGroup>();

            return specification;
        }

        public static Specification<MessageGroup> MessageGroupFullText(string text)
        {
            Specification<MessageGroup> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var descriptionSpec = new DirectSpecification<MessageGroup>(c => c.Description.Contains(text));

                specification &= (descriptionSpec);
            }

            return specification;
        }
    }
}
