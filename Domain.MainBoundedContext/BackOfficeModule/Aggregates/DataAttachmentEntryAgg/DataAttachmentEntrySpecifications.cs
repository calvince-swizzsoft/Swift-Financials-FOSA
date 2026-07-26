using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentEntryAgg
{
    public static class DataAttachmentEntrySpecifications
    {
        public static Specification<DataAttachmentEntry> DefaultSpec()
        {
            Specification<DataAttachmentEntry> specification = new TrueSpecification<DataAttachmentEntry>();

            return specification;
        }

        public static ISpecification<DataAttachmentEntry> DataAttachmentEntryWithDataAttachmentPeriodIdAndCustomerAccountId(Guid dataAttachmentPeriodId, Guid customerAccountId)
        {
            Specification<DataAttachmentEntry> specification = DefaultSpec();

            if (dataAttachmentPeriodId != null && dataAttachmentPeriodId != Guid.Empty && customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<DataAttachmentEntry>(x => x.DataAttachmentPeriodId == dataAttachmentPeriodId && x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }

        public static Specification<DataAttachmentEntry> DataAttachmentEntryFullText(Guid dataAttachmentPeriodId, string text)
        {
            Specification<DataAttachmentEntry> specification = new DirectSpecification<DataAttachmentEntry>(x => x.DataAttachmentPeriodId == dataAttachmentPeriodId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<DataAttachmentEntry>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                var createdBySpec = new DirectSpecification<DataAttachmentEntry>(c => c.CreatedBy.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec | createdBySpec);
            }

            return specification;
        }
    }
}
