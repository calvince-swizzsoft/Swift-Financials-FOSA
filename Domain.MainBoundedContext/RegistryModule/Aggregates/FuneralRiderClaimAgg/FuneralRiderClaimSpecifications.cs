using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FuneralRiderClaimAgg
{
    public static class FuneralRiderClaimSpecifications
    {
        public static Specification<FuneralRiderClaim> DefaultSpec()
        {
            Specification<FuneralRiderClaim> specification = new TrueSpecification<FuneralRiderClaim>();

            return specification;
        }

        public static Specification<FuneralRiderClaim> FindFuneralRiderClaimWithCustomerId(Guid customerId)
        {
            Specification<FuneralRiderClaim> specification = new DirectSpecification<FuneralRiderClaim>(x => x.CustomerId == customerId);

            return specification;
        }

        public static Specification<FuneralRiderClaim> FuneralRiderClaimFullText(string text)
        {
            Specification<FuneralRiderClaim> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var customerIdSpec = new DirectSpecification<FuneralRiderClaim>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var claimantIdSpec = new DirectSpecification<FuneralRiderClaim>(c => c.FuneralRiderClaimant.IdentityCardNumber.Contains(text));
                var claimantNameSpec = new DirectSpecification<FuneralRiderClaim>(c => c.FuneralRiderClaimant.Name.Contains(text));
                var customerNameSpec = new DirectSpecification<FuneralRiderClaim>(c => c.Customer.Individual.FirstName.Contains(text));
                var customerLastNameSpec = new DirectSpecification<FuneralRiderClaim>(c => c.Customer.Individual.LastName.Contains(text));
                var tscNoSpec = new DirectSpecification<FuneralRiderClaim>(c => c.FuneralRiderClaimant.TscNumber.Contains(text));

                specification &= (customerIdSpec | claimantIdSpec | claimantNameSpec | customerNameSpec | customerLastNameSpec | tscNoSpec);
            }

            return specification;
        }

        public static Specification<FuneralRiderClaim> FuneralRiderClaimByStatusWithDateRange(int status, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FuneralRiderClaim> specification = new DirectSpecification<FuneralRiderClaim>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = FuneralRiderClaimFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }

        public static Specification<FuneralRiderClaim> FuneralRiderClaimByStatusWithFilter(int status, string text)
        {
            Specification<FuneralRiderClaim> specification = new DirectSpecification<FuneralRiderClaim>(x => x.Status == status);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = FuneralRiderClaimFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }

        public static Specification<FuneralRiderClaim> FuneralRiderClaimByTypeStatusWithDateRange(int status, int claimType, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FuneralRiderClaim> specification = new DirectSpecification<FuneralRiderClaim>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status && x.ClaimType == claimType);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = FuneralRiderClaimFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }

        public static Specification<FuneralRiderClaim> FindFuneralRiderClaimWithCustomerIdAndType(Guid customerId, int type)
        {
            Specification<FuneralRiderClaim> specification = new DirectSpecification<FuneralRiderClaim>(x => x.CustomerId == customerId && x.ClaimType == type);

            return specification;
        }

        public static Specification<FuneralRiderClaim> FindFuneralRiderClaimByCustomerIdTypeAndClaimantIdentity(Guid customerId, int type, string claimIdentity)
        {
            Specification<FuneralRiderClaim> specification = new DirectSpecification<FuneralRiderClaim>(x => x.CustomerId == customerId && x.ClaimType == type && x.FuneralRiderClaimant.IdentityCardNumber == claimIdentity);

            return specification;
        }
    }
}
