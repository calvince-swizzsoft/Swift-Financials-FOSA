using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.InterAccountTransferBatchEntryAgg
{
    public static class InterAccountTransferBatchEntrySpecifications
    {
        public static Specification<InterAccountTransferBatchEntry> DefaultSpec()
        {
            Specification<InterAccountTransferBatchEntry> specification = new TrueSpecification<InterAccountTransferBatchEntry>();

            return specification;
        }

        public static Specification<InterAccountTransferBatchEntry> InterAccountTransferBatchEntryWithInterAccountTransferBatchId(Guid interAccountTransferBatchId, string text)
        {
            Specification<InterAccountTransferBatchEntry> specification = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.InterAccountTransferBatchId == interAccountTransferBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nonIndividualSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                var referenceSpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<InterAccountTransferBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                | referenceSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<InterAccountTransferBatchEntry> PostedInterAccountTransferBatchEntryWithInterAccountTransferBatchId(Guid interAccountTransferBatchId)
        {
            Specification<InterAccountTransferBatchEntry> specification = new DirectSpecification<InterAccountTransferBatchEntry>(x => x.InterAccountTransferBatchId == interAccountTransferBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }
    }
}
