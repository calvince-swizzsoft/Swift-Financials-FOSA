using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ARCustomerAgg
{
    public class ARCustomer : Entity
    {
        public string No { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string MobilePhoneNumber { get; set; }

        public string Town { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        public string ContactPersonName { get; set; }
        public string ContactPersonPhoneNo { get; set; }
    }
}
