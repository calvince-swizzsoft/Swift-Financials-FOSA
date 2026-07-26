using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.CustomerMessageHistoryAgg
{
    public static class CustomerMessageHistorySpecifications
    {
        public static Specification<CustomerMessageHistory> DefaultSpec()
        {
            Specification<CustomerMessageHistory> specification = new TrueSpecification<CustomerMessageHistory>();

            return specification;
        }
    }
}
