using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FuneralRiderClaimPayable
{
    public static class FuneralRiderClaimPayableSpecifications
    {
        public static Specification<FuneralRiderClaimPayable> DefaultSpec()
        {
            Specification<FuneralRiderClaimPayable> specification = new TrueSpecification<FuneralRiderClaimPayable>();

            return specification;
        }

        public static Specification<FuneralRiderClaimPayable> FuneralRiderClaimId(Guid funeralRiderClaimId)
        {
            Specification<FuneralRiderClaimPayable> specification = new DirectSpecification<FuneralRiderClaimPayable>(x => x.FuneralRiderClaimId == funeralRiderClaimId);

            return specification;
        }

        public static Specification<FuneralRiderClaimPayable> FuneralRiderClaimPayableFullText(string text)
        {
            Specification<FuneralRiderClaimPayable> specification = DefaultSpec();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var customerIdSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.FuneralRiderClaim.Customer.Individual.IdentityCardNumber.Contains(text));
                var claimantIdSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.FuneralRiderClaim.FuneralRiderClaimant.IdentityCardNumber.Contains(text));
                var claimantNameSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.FuneralRiderClaim.FuneralRiderClaimant.Name.Contains(text));
                var customerNameSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.FuneralRiderClaim.Customer.Individual.FirstName.Contains(text));
                var customerLastNameSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.FuneralRiderClaim.Customer.Individual.LastName.Contains(text));
                var tscNoSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.FuneralRiderClaim.FuneralRiderClaimant.TscNumber.Contains(text));

                specification &= (customerIdSpec | claimantIdSpec | claimantNameSpec | customerNameSpec | customerLastNameSpec | tscNoSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var refNumberSpec = new DirectSpecification<FuneralRiderClaimPayable>(c => c.ReferenceNumber == number);

                    specification |= refNumberSpec;
                }
            }

            return specification;
        }

        public static Specification<FuneralRiderClaimPayable> FuneralRiderClaimPayableWithStatusAndDateRange(int status, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FuneralRiderClaimPayable> specification = new DirectSpecification<FuneralRiderClaimPayable>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.RecordStatus == status);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = FuneralRiderClaimPayableFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }

        public static Specification<FuneralRiderClaimPayable> FuneralRiderClaimPayableWithRecordStatusPaymentStatusAndDateRange(int status, int paymentStatus, string text, DateTime startDate, DateTime endDate)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FuneralRiderClaimPayable> specification = new DirectSpecification<FuneralRiderClaimPayable>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.RecordStatus == status && x.PaymentStatus == paymentStatus);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = FuneralRiderClaimPayableFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }

        public static Specification<FuneralRiderClaimPayable> FuneralRiderClaimWithRecordStatusPaymentStatusAndFilter(int recordStatus, int paymentStatus, string text)
        {
            Specification<FuneralRiderClaimPayable> specification = new DirectSpecification<FuneralRiderClaimPayable>(x => x.RecordStatus == recordStatus && x.PaymentStatus == paymentStatus);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var fullTextSpec = FuneralRiderClaimPayableFullText(text);

                specification &= fullTextSpec;
            }

            return specification;
        }

    }
}
