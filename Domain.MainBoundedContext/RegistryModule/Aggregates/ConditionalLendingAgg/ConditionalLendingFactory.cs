using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.ConditionalLendingAgg
{
    public static class ConditionalLendingFactory
    {
        public static ConditionalLending CreateConditionalLending(Guid loanProductId, string description)
        {
            var conditionalLending = new ConditionalLending();

            conditionalLending.GenerateNewIdentity();

            conditionalLending.LoanProductId = loanProductId;

            conditionalLending.Description = description;

            conditionalLending.CreatedDate = DateTime.Now;

            return conditionalLending;
        }
    }
}
