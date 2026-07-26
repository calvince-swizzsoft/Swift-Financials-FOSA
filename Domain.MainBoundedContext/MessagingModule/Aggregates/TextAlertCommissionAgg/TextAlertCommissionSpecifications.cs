using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MessagingModule.Aggregates.TextAlertCommissionAgg
{
    public static class TextAlertCommissionSpecifications
    {
        public static Specification<TextAlertCommission> DefaultSpec()
        {
            Specification<TextAlertCommission> specification = new TrueSpecification<TextAlertCommission>();

            return specification;
        }

        public static ISpecification<TextAlertCommission> TextAlertCommission(int systemTransactionCode)
        {
            Specification<TextAlertCommission> specification = new TrueSpecification<TextAlertCommission>();

            specification &= new DirectSpecification<TextAlertCommission>(x => x.SystemTransactionCode == systemTransactionCode);

            return specification;
        }

        public static Specification<TextAlertCommission> SystemTransactionCodeAndCommissionId(int systemTransactionCode, Guid commissionId)
        {
            Specification<TextAlertCommission> specification =
                new DirectSpecification<TextAlertCommission>(m => m.SystemTransactionCode == systemTransactionCode && m.CommissionId == commissionId);

            return specification;
        }
    }
}
