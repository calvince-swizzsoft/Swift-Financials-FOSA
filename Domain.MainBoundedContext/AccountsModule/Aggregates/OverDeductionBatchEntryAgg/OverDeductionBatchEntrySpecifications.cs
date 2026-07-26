using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchEntryAgg
{
    public static class OverDeductionBatchEntrySpecifications
    {
        public static Specification<OverDeductionBatchEntry> DefaultSpec()
        {
            Specification<OverDeductionBatchEntry> specification = new TrueSpecification<OverDeductionBatchEntry>();

            return specification;
        }

        public static Specification<OverDeductionBatchEntry> OverDeductionBatchEntryWithOverDeductionBatchId(Guid overDeductionBatchId, string text)
        {
            Specification<OverDeductionBatchEntry> specification = new DirectSpecification<OverDeductionBatchEntry>(c => c.OverDeductionBatchId == overDeductionBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nonIndividualSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.DebitCustomerAccount.Customer.Address.MobileLine.Contains(text));

                var createdBySpec = new DirectSpecification<OverDeductionBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                | createdBySpec);
            }

            return specification;
        }

        public static Specification<OverDeductionBatchEntry> PostedOverDeductionBatchEntryWithOverDeductionBatchId(Guid overDeductionBatchId)
        {
            Specification<OverDeductionBatchEntry> specification = new DirectSpecification<OverDeductionBatchEntry>(x => x.OverDeductionBatchId == overDeductionBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }
    }
}
