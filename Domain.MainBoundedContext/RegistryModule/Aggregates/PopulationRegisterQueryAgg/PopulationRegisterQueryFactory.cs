using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.PopulationRegisterQueryAgg
{
    public static class PopulationRegisterQueryFactory
    {
        public static PopulationRegisterQuery CreatePopulationRegisterQuery(Guid branchId, Guid customerId, int identityType, string identityNumber, string identitySerialNumber, string remarks)
        {
            var populationRegisterQuery = new PopulationRegisterQuery();

            populationRegisterQuery.GenerateNewIdentity();

            populationRegisterQuery.BranchId = branchId;

            populationRegisterQuery.CustomerId = customerId;

            populationRegisterQuery.IdentityType = (byte)identityType;

            populationRegisterQuery.IdentityNumber = identityNumber;

            populationRegisterQuery.IdentitySerialNumber = identitySerialNumber;

            populationRegisterQuery.Remarks = remarks;

            populationRegisterQuery.CreatedDate = DateTime.Now;

            return populationRegisterQuery;
        }
    }
}
