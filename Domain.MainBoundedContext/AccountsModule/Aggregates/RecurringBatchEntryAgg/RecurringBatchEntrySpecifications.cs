using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.RecurringBatchEntryAgg
{
    public static class RecurringBatchEntrySpecifications
    {
        public static Specification<RecurringBatchEntry> DefaultSpec()
        {
            Specification<RecurringBatchEntry> specification = new TrueSpecification<RecurringBatchEntry>();

            return specification;
        }

        public static Specification<RecurringBatchEntry> RecurringBatchEntryWithRecurringBatchId(Guid recurringBatchId, string text)
        {
            Specification<RecurringBatchEntry> specification = new DirectSpecification<RecurringBatchEntry>(c => c.RecurringBatchId == recurringBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nonIndividualSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<RecurringBatchEntry>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                var srcFirstNameSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Individual.FirstName.Contains(text));
                var srcLastNameSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Individual.LastName.Contains(text));
                var srcNonIndividualDescriptionSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.NonIndividual.Description.Contains(text));
                var srcPayrollNumbersSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var srcIdentificationNumberSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var srcReference1Spec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Reference1.Contains(text));
                var srcReference2Spec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Reference2.Contains(text));
                var srcReference3Spec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BenefactorCustomerAccount.Customer.Reference3.Contains(text));
                var srcRemarksSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.Remarks.Contains(text));

                var destFirstNameSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.Individual.FirstName.Contains(text));
                var destLastNameSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.Individual.LastName.Contains(text));
                var destNonIndividualDescriptionSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.NonIndividual.Description.Contains(text));
                var destPayrollNumbersSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var destIdentificationNumberSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var destReference1Spec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.Reference1.Contains(text));
                var destReference2Spec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.BeneficiaryCustomerAccount.Customer.Reference2.Contains(text));
                var destRemarksSpec = new DirectSpecification<RecurringBatchEntry>(c => c.StandingOrder.Remarks.Contains(text));

                var remarksSpec = new DirectSpecification<RecurringBatchEntry>(c => c.Remarks.Contains(text));
                var referenceSpec = new DirectSpecification<RecurringBatchEntry>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<RecurringBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                | srcFirstNameSpec | srcLastNameSpec | srcNonIndividualDescriptionSpec | srcPayrollNumbersSpec | srcIdentificationNumberSpec | srcReference1Spec | srcReference2Spec | srcReference3Spec | srcRemarksSpec
                | destFirstNameSpec | destLastNameSpec | destNonIndividualDescriptionSpec | destPayrollNumbersSpec | destIdentificationNumberSpec | destReference1Spec | destReference2Spec | destRemarksSpec
                | remarksSpec | referenceSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<RecurringBatchEntry> PostedRecurringBatchEntryWithRecurringBatchId(Guid recurringBatchId)
        {
            Specification<RecurringBatchEntry> specification = new DirectSpecification<RecurringBatchEntry>(x => x.RecurringBatchId == recurringBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }

        public static Specification<RecurringBatchEntry> QueableRecurringBatchEntries()
        {
            Specification<RecurringBatchEntry> specification = new DirectSpecification<RecurringBatchEntry>(c => c.Status == (int)BatchEntryStatus.Pending && c.RecurringBatch.Status == (int)BatchStatus.Posted);
            
            return specification;
        }
    }
}
