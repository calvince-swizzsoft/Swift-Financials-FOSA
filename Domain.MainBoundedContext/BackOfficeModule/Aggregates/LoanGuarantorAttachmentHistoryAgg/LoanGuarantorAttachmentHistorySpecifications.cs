using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanGuarantorAttachmentHistoryAgg
{
    public static class LoanGuarantorAttachmentHistorySpecifications
    {
        public static Specification<LoanGuarantorAttachmentHistory> DefaultSpec()
        {
            Specification<LoanGuarantorAttachmentHistory> specification = new TrueSpecification<LoanGuarantorAttachmentHistory>();

            return specification;
        }

        public static Specification<LoanGuarantorAttachmentHistory> LoanGuarantorAttachmentHistoryWithStatusAndFullText(DateTime startDate, DateTime endDate, int status, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanGuarantorAttachmentHistory> specification = DefaultSpec();

            specification &= new DirectSpecification<LoanGuarantorAttachmentHistory>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var customerReference1Spec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Reference1.Contains(text));
                var customerReference2Spec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Reference2.Contains(text));
                var customerReference3Spec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<LoanGuarantorAttachmentHistory>(c => c.SourceCustomerAccount.Customer.Address.MobileLine.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | customerReference1Spec | customerReference2Spec | customerReference3Spec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }
    }
}
