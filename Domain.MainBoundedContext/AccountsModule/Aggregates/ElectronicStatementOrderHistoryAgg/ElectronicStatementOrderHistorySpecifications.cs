using Domain.Seedwork.Specification;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderHistoryAgg
{
    public static class ElectronicStatementOrderHistorySpecifications
    {
        public static Specification<ElectronicStatementOrderHistory> DefaultSpec()
        {
            Specification<ElectronicStatementOrderHistory> specification = new TrueSpecification<ElectronicStatementOrderHistory>();

            return specification;
        }

        public static Specification<ElectronicStatementOrderHistory> ElectronicStatementOrderHistoryWithElectronicStatementOrderId(Guid electronicStatementOrderId)
        {
            Specification<ElectronicStatementOrderHistory> specification = DefaultSpec();

            if (electronicStatementOrderId != null && electronicStatementOrderId != Guid.Empty)
            {
                var electronicStatementOrderIdSpec = new DirectSpecification<ElectronicStatementOrderHistory>(c => c.ElectronicStatementOrderId == electronicStatementOrderId);

                specification &= electronicStatementOrderIdSpec;
            }

            return specification;
        }
    }
}
