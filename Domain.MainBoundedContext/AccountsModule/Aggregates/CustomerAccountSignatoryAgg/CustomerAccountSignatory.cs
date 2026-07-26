using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.MainBoundedContext.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountSignatoryAgg
{
    public class CustomerAccountSignatory : Domain.Seedwork.Entity
    {
        public Guid CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public int Salutation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int IdentityCardType { get; set; }

        public string IdentityCardNumber { get; set; }

        public int Gender { get; set; }

        public int Relationship { get; set; }

        public virtual Address Address { get; set; }

        public string Remarks { get; set; }

        

        
    }
}
