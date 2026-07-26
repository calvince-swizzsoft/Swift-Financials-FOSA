using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.PaySlipAgg
{
    public static class PaySlipSpecifications
    {
        public static Specification<PaySlip> DefaultSpec()
        {
            Specification<PaySlip> specification = new TrueSpecification<PaySlip>();

            return specification;
        }

        public static ISpecification<PaySlip> PaySlipWithSalaryPeriodIdAndMonthAndEmployeeId(Guid salaryPeriodId, int month, Guid employeeId)
        {
            Specification<PaySlip> specification = new DirectSpecification<PaySlip>(x => x.SalaryPeriodId == salaryPeriodId && x.SalaryPeriod.Month == month && x.SalaryCardId == employeeId);

            return specification;
        }

        public static ISpecification<PaySlip> PaySlipWithSalaryPeriodId(Guid salaryPeriodId)
        {
            Specification<PaySlip> specification = new DirectSpecification<PaySlip>(x => x.SalaryPeriodId == salaryPeriodId);

            return specification;
        }

        public static Specification<PaySlip> PaySlipFullText(Guid salaryPeriodId, string text)
        {
            Specification<PaySlip> specification = new DirectSpecification<PaySlip>(x => x.SalaryPeriodId == salaryPeriodId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var personalIdentificationNumberSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.PersonalIdentificationNumber.Contains(text));
                var nationalSocialSecurityFundNumberSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.NationalSocialSecurityFundNumber.Contains(text));
                var nationalHospitalInsuranceFundNumberSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.NationalHospitalInsuranceFundNumber.Contains(text));

                var firstNameSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<PaySlip>(c => c.SalaryCard.Employee.Customer.Address.MobileLine.Contains(text));

                specification &= (personalIdentificationNumberSpec | nationalSocialSecurityFundNumberSpec | nationalHospitalInsuranceFundNumberSpec
                    | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }

        public static ISpecification<PaySlip> PaySlipWithDateRangeAndCustomerId(DateTime startDate, DateTime endDate, Guid customerId)
        {
            Specification<PaySlip> specification = DefaultSpec();

            if (startDate != null && endDate != null)
            {
                endDate = UberUtil.AdjustTimeSpan(endDate);

                specification &= new DirectSpecification<PaySlip>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.SalaryCard.Employee.CustomerId == customerId);
            }

            return specification;
        }

        public static Specification<PaySlip> PostedPaySlipWithSalaryPeriodId(Guid salaryPeriodId)
        {
            Specification<PaySlip> specification = new DirectSpecification<PaySlip>(x => x.SalaryPeriodId == salaryPeriodId && x.Status == (int)PaySlipStatus.Posted);

            return specification;
        }

        public static Specification<PaySlip> QueablePaySlips()
        {
            Specification<PaySlip> specification = new DirectSpecification<PaySlip>(c => c.Status == (int)PaySlipStatus.Pending && c.SalaryPeriod.Status == (int)SalaryPeriodStatus.Closed);

            return specification;
        }
    }
}
