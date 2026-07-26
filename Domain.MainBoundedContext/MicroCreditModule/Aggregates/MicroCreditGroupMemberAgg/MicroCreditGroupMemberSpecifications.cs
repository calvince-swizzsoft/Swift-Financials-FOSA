using Domain.Seedwork.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.MicroCreditModule.Aggregates.MicroCreditGroupMemberAgg
{
    public static class MicroCreditGroupMemberSpecifications
    {
        public static Specification<MicroCreditGroupMember> DefaultSpec()
        {
            Specification<MicroCreditGroupMember> specification = new TrueSpecification<MicroCreditGroupMember>();

            return specification;
        }

        public static ISpecification<MicroCreditGroupMember> MicroCreditGroupMemberWithMicroCreditGroupId(Guid microCreditGroupId)
        {
            Specification<MicroCreditGroupMember> specification = new TrueSpecification<MicroCreditGroupMember>();

            if (microCreditGroupId != null && microCreditGroupId != Guid.Empty)
            {
                specification &= new DirectSpecification<MicroCreditGroupMember>(x => x.MicroCreditGroupId == microCreditGroupId);
            }

            return specification;
        }

        public static ISpecification<MicroCreditGroupMember> MicroCreditGroupMemberWithMicroCreditGroupCustomerId(Guid microCreditGroupCustomerId)
        {
            Specification<MicroCreditGroupMember> specification = new TrueSpecification<MicroCreditGroupMember>();

            if (microCreditGroupCustomerId != null && microCreditGroupCustomerId != Guid.Empty)
            {
                specification &= new DirectSpecification<MicroCreditGroupMember>(x => x.MicroCreditGroup.CustomerId == microCreditGroupCustomerId);
            }

            return specification;
        }

        public static ISpecification<MicroCreditGroupMember> MicroCreditGroupMemberWithCustomerId(Guid customerId)
        {
            Specification<MicroCreditGroupMember> specification = new TrueSpecification<MicroCreditGroupMember>();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<MicroCreditGroupMember>(x => x.CustomerId == customerId);
            }

            return specification;
        }

        public static ISpecification<MicroCreditGroupMember> MicroCreditGroupMemberWithCustomerIdAndMicroCreditGroupWithCustomerId(Guid microCreditGroupMemberCustomerId, Guid microCreditGroupCustomerId)
        {
            Specification<MicroCreditGroupMember> specification = new TrueSpecification<MicroCreditGroupMember>();

            if (microCreditGroupMemberCustomerId != null && microCreditGroupMemberCustomerId != Guid.Empty && microCreditGroupCustomerId != null && microCreditGroupCustomerId != Guid.Empty)
            {
                specification &= new DirectSpecification<MicroCreditGroupMember>(x => x.CustomerId == microCreditGroupMemberCustomerId && x.MicroCreditGroup.CustomerId == microCreditGroupCustomerId);
            }

            return specification;
        }

        public static ISpecification<MicroCreditGroupMember> MicroCreditGroupMemberWithMicroCreditGroupIdAndDesignation(Guid microCreditGroupId, int designation)
        {
            Specification<MicroCreditGroupMember> specification = new TrueSpecification<MicroCreditGroupMember>();

            if (microCreditGroupId != null && microCreditGroupId != Guid.Empty)
            {
                specification &= new DirectSpecification<MicroCreditGroupMember>(x => x.MicroCreditGroupId == microCreditGroupId && x.Designation == designation);
            }

            return specification;
        }

        public static Specification<MicroCreditGroupMember> MicroCreditGroupMemberFullText(Guid microCreditGroupId, string text)
        {
            Specification<MicroCreditGroupMember> specification = new DirectSpecification<MicroCreditGroupMember>(x => x.MicroCreditGroupId == microCreditGroupId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var firstNameSpec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<MicroCreditGroupMember>(c => c.Customer.Address.MobileLine.Contains(text));

                var createdBySpec = new DirectSpecification<MicroCreditGroupMember>(c => c.CreatedBy.Contains(text));

                specification &= (firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec | createdBySpec);
            }

            return specification;
        }
    }
}
