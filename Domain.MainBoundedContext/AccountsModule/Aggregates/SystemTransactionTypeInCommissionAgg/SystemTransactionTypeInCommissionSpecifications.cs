using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.SystemTransactionTypeInCommissionAgg
{
    public static class SystemTransactionTypeInCommissionSpecifications
    {
        public static Specification<SystemTransactionTypeInCommission> SystemTransactionType(int systemTransactionType)
        {
            Specification<SystemTransactionTypeInCommission> specification =
                new DirectSpecification<SystemTransactionTypeInCommission>(m => m.SystemTransactionType == systemTransactionType);

            return specification;
        }

        public static Specification<SystemTransactionTypeInCommission> SystemTransactionTypeAndCommissionId(int systemTransactionType, Guid commissionId)
        {
            Specification<SystemTransactionTypeInCommission> specification =
                new DirectSpecification<SystemTransactionTypeInCommission>(m => m.SystemTransactionType == systemTransactionType && m.CommissionId == commissionId);

            return specification;
        }
    }
}
