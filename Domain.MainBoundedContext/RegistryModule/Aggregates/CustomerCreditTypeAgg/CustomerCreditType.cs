using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerCreditTypeAgg
{
    public class CustomerCreditType : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }

        public Guid CreditTypeId { get; set; }

        public virtual CreditType CreditType { get; private set; }

        
        
        
    }
}
