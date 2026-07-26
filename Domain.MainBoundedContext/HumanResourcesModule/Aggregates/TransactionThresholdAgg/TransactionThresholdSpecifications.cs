using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TransactionThresholdAgg
{
    public static class TransactionThresholdSpecifications
    {
        public static Specification<TransactionThreshold> DefaultSpec()
        {
            Specification<TransactionThreshold> specification = new TrueSpecification<TransactionThreshold>();

            return specification;
        }

        public static ISpecification<TransactionThreshold> TransactionThresholdWithDesignationId(Guid designationId)
        {
            Specification<TransactionThreshold> specification = DefaultSpec();

            if (designationId != null && designationId != Guid.Empty)
            {
                specification &= new DirectSpecification<TransactionThreshold>(x => x.DesignationId == designationId);
            }

            return specification;
        }

        public static ISpecification<TransactionThreshold> TransactionThresholdWithDesignationIdAndType(Guid designationId, int type)
        {
            Specification<TransactionThreshold> specification = DefaultSpec();

            if (designationId != null && designationId != Guid.Empty)
            {
                specification &= new DirectSpecification<TransactionThreshold>(x => x.DesignationId == designationId && x.Type == type);
            }

            return specification;
        }
    }
}
