using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg
{
    public static class LoanDisbursementBatchEntrySpecifications
    {
        public static Specification<LoanDisbursementBatchEntry> DefaultSpec()
        {
            Specification<LoanDisbursementBatchEntry> specification = new TrueSpecification<LoanDisbursementBatchEntry>();

            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> LoanDisbursementBatchEntryWithLoanDisbursementBatchId(Guid loanDisbursementBatchId, string text)
        {
            Specification<LoanDisbursementBatchEntry> specification = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanDisbursementBatchId == loanDisbursementBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var nonIndividualSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanCase.Customer.Address.MobileLine.Contains(text));

                var referenceSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                | referenceSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> PostedLoanDisbursementBatchEntryWithLoanDisbursementBatchId(Guid loanDisbursementBatchId)
        {
            Specification<LoanDisbursementBatchEntry> specification = new DirectSpecification<LoanDisbursementBatchEntry>(x => x.LoanDisbursementBatchId == loanDisbursementBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> LoanDisbursementBatchEntryWithLoanDisbursementBatchTypeAndCustomerId(int loanDisbursementBatchType, Guid customerId)
        {
            Specification<LoanDisbursementBatchEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var loanDisbursementBatchIdSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanDisbursementBatch.Type == loanDisbursementBatchType && c.LoanCase.CustomerId == customerId);

                specification &= loanDisbursementBatchIdSpec;
            }

            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> QueableLoanDisbursementBatchEntries()
        {
            Specification<LoanDisbursementBatchEntry> specification = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.Status == (int)BatchEntryStatus.Pending && c.LoanDisbursementBatch.Status == (int)BatchStatus.Posted);
            
            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> LoanDisbursementBatchEntryWithDateRangeAndLoanDisbursementBatchType(DateTime startDate, DateTime endDate, int loanDisbursementBatchType, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanDisbursementBatchEntry> specification = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate && c.LoanDisbursementBatch.Type == loanDisbursementBatchType);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanDisbursementBatch.Branch.Description.Contains(text));
                var referenceSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanDisbursementBatch.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.CreatedBy.Contains(text));

                var entryReferenceSpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.Reference.Contains(text));
                var entryCreatedBySpec = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.CreatedBy.Contains(text));

                specification &= (entryCreatedBySpec | branchSpec | referenceSpec | entryReferenceSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> LoanDisbursementBatchEntryWithLoanCaseId(Guid loanCaseId)
        {
            Specification<LoanDisbursementBatchEntry> specification = new DirectSpecification<LoanDisbursementBatchEntry>(x => x.LoanCaseId == loanCaseId);

            return specification;
        }

        public static Specification<LoanDisbursementBatchEntry> LoanDisbursementBatchEntryExceedingThreshold(Guid loanDisbursementBatchId, decimal threshold)
        {
            Specification<LoanDisbursementBatchEntry> specification = new DirectSpecification<LoanDisbursementBatchEntry>(c => c.LoanDisbursementBatchId == loanDisbursementBatchId && c.LoanCase.ApprovedAmount >= threshold);

            return specification;
        }
    }
}
