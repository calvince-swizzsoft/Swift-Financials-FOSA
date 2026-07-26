using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg
{
    public static class FixedDepositSpecifications
    {
        public static Specification<FixedDeposit> DefaultSpec()
        {
            Specification<FixedDeposit> specification = new TrueSpecification<FixedDeposit>();

            return specification;
        }

        public static Specification<FixedDeposit> FixedDepositsWithBranchId(Guid branchId)
        {
            Specification<FixedDeposit> specification = new DirectSpecification<FixedDeposit>(x => x.BranchId == branchId);

            return specification;
        }

        public static Specification<FixedDeposit> PayableFixedDepositsWithMaturityDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FixedDeposit> specification = new DirectSpecification<FixedDeposit>(x => x.Status == (int)FixedDepositStatus.Running && x.MaturityDate >= startDate && x.MaturityDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<FixedDeposit>(c => c.Branch.Description.Contains(text));
                var remarksSpec = new DirectSpecification<FixedDeposit>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<FixedDeposit>(c => c.CreatedBy.Contains(text));
                var paidBySpec = new DirectSpecification<FixedDeposit>(c => c.PaidBy.Contains(text));

                var payrollNumbersSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (branchSpec | remarksSpec | createdBySpec | paidBySpec | payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 |
                     addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static Specification<FixedDeposit> RevocableFixedDepositsWithMaturityDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FixedDeposit> specification = new DirectSpecification<FixedDeposit>(x => x.Status == (int)FixedDepositStatus.Running && x.MaturityDate >= startDate && x.MaturityDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<FixedDeposit>(c => c.Branch.Description.Contains(text));
                var remarksSpec = new DirectSpecification<FixedDeposit>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<FixedDeposit>(c => c.CreatedBy.Contains(text));
                var paidBySpec = new DirectSpecification<FixedDeposit>(c => c.PaidBy.Contains(text));

                var payrollNumbersSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (branchSpec | remarksSpec | createdBySpec | paidBySpec | payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 |
                     addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static ISpecification<FixedDeposit> FixedDepositFullText(string text)
        {
            Specification<FixedDeposit> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<FixedDeposit>(c => c.Branch.Description.Contains(text));
                var remarksSpec = new DirectSpecification<FixedDeposit>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<FixedDeposit>(c => c.CreatedBy.Contains(text));
                var paidBySpec = new DirectSpecification<FixedDeposit>(c => c.PaidBy.Contains(text));

                var payrollNumbersSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (branchSpec | remarksSpec | createdBySpec | paidBySpec | payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 |
                     addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static Specification<FixedDeposit> FixedDepositWithStatusAndFullText(int status, string text)
        {
            Specification<FixedDeposit> specification = new DirectSpecification<FixedDeposit>(x => x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var branchSpec = new DirectSpecification<FixedDeposit>(c => c.Branch.Description.Contains(text));
                var remarksSpec = new DirectSpecification<FixedDeposit>(c => c.Remarks.Contains(text));
                var createdBySpec = new DirectSpecification<FixedDeposit>(c => c.CreatedBy.Contains(text));
                var paidBySpec = new DirectSpecification<FixedDeposit>(c => c.PaidBy.Contains(text));

                var payrollNumbersSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<FixedDeposit>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (branchSpec | remarksSpec | createdBySpec | paidBySpec | payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 |
                     addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static Specification<FixedDeposit> FixedDepositsWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<FixedDeposit> specification = new DirectSpecification<FixedDeposit>(x => x.CustomerAccountId == customerAccountId && x.Status == (int)FixedDepositStatus.Running);

            return specification;
        }

        public static Specification<FixedDeposit> DueFixedDeposits(DateTime targetDate)
        {
            Specification<FixedDeposit> specification = new DirectSpecification<FixedDeposit>(x => x.Status == (int)FixedDepositStatus.Running && DateTime.Compare(x.MaturityDate, targetDate) == 0);

            return specification;
        }
    }
}
