using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.GraduatedScaleAgg
{
    public static class GraduatedScaleSpecifications
    {
        public static Specification<GraduatedScale> DefaultSpec()
        {
            Specification<GraduatedScale> specification = new TrueSpecification<GraduatedScale>();

            return specification;
        }

        public static ISpecification<GraduatedScale> GraduatedScaleWithCommissionId(Guid commissionId)
        {
            Specification<GraduatedScale> specification = new TrueSpecification<GraduatedScale>();

            if (commissionId != null && commissionId != Guid.Empty)
            {
                specification &= new DirectSpecification<GraduatedScale>(x => x.CommissionId == commissionId);
            }

            return specification;
        }
    }
}
