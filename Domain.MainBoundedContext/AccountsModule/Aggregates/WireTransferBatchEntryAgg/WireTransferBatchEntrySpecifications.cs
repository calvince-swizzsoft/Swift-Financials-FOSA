using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.WireTransferBatchEntryAgg
{
    public static class WireTransferBatchEntrySpecifications
    {
        public static Specification<WireTransferBatchEntry> DefaultSpec()
        {
            Specification<WireTransferBatchEntry> specification = new TrueSpecification<WireTransferBatchEntry>();

            return specification;
        }

        public static Specification<WireTransferBatchEntry> WireTransferBatchEntryWithWireTransferBatchId(Guid wireTransferBatchId, string text)
        {
            Specification<WireTransferBatchEntry> specification = new DirectSpecification<WireTransferBatchEntry>(c => c.WireTransferBatchId == wireTransferBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nonIndividualSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                var entryPayeeSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.Payee.Contains(text));
                var entryAccountNumberSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.AccountNumber.Contains(text));
                var referenceSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                | entryPayeeSpec | entryAccountNumberSpec | referenceSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<WireTransferBatchEntry> PostedWireTransferBatchEntryWithWireTransferBatchId(Guid wireTransferBatchId)
        {
            Specification<WireTransferBatchEntry> specification = new DirectSpecification<WireTransferBatchEntry>(x => x.WireTransferBatchId == wireTransferBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }

        public static Specification<WireTransferBatchEntry> QueableWireTransferBatchEntries()
        {
            Specification<WireTransferBatchEntry> specification = new DirectSpecification<WireTransferBatchEntry>(c => c.Status == (int)BatchEntryStatus.Pending && c.WireTransferBatch.Status == (int)BatchStatus.Posted);

            return specification;
        }

        public static Specification<WireTransferBatchEntry> WireTransferBatchEntryWithDateRangeAndWireTransferBatchType(DateTime startDate, DateTime endDate, int creditBatchType, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<WireTransferBatchEntry> specification = new DirectSpecification<WireTransferBatchEntry>(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate && c.WireTransferBatch.Type == creditBatchType);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.WireTransferBatch.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.WireTransferBatch.Reference.Contains(text));
                var wireTransferTypeSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.WireTransferBatch.WireTransferType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CreatedBy.Contains(text));

                var entryPayeeSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.Payee.Contains(text));
                var entryAccountNumberSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.AccountNumber.Contains(text));
                var entryReferenceSpec = new DirectSpecification<WireTransferBatchEntry>(c => c.Reference.Contains(text));
                var entryCreatedBySpec = new DirectSpecification<WireTransferBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (entryPayeeSpec | entryAccountNumberSpec | entryReferenceSpec | entryCreatedBySpec | branchSpec | referenceSpec | wireTransferTypeSpec | createdBySpec);
            }

            return specification;
        }
    }
}
