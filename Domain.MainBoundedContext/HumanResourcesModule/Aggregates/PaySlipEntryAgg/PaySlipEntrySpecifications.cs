using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipEntryAgg
{
    public static class PaySlipEntrySpecifications
    {
        public static Specification<PaySlipEntry> DefaultSpec()
        {
            Specification<PaySlipEntry> specification = new TrueSpecification<PaySlipEntry>();

            return specification;
        }

        public static ISpecification<PaySlipEntry> PaySlipEntryWithPaySlipId(Guid paySlipId)
        {
            Specification<PaySlipEntry> specification = DefaultSpec();

            if (paySlipId != null && paySlipId != Guid.Empty)
            {
                specification &= new DirectSpecification<PaySlipEntry>(x => x.PaySlipId == paySlipId);
            }

            return specification;
        }
    }
}
