using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeBookAgg
{
    public static class ChequeBookSpecifications
    {
        public static Specification<ChequeBook> DefaultSpec()
        {
            Specification<ChequeBook> specification = new TrueSpecification<ChequeBook>();

            return specification;
        }

        public static ISpecification<ChequeBook> ChequeBookWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<ChequeBook> specification = new TrueSpecification<ChequeBook>();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<ChequeBook>(x => x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }

        public static Specification<ChequeBook> ChequeBookFullText(string text)
        {
            Specification<ChequeBook> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var membershipNumberSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.SerialNumber == number);

                    var payrollNumbersSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));

                    var addressLandLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                    var reference1Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                    var reference2Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                    var reference3Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                    var referenceSpec = new DirectSpecification<ChequeBook>(c => c.Reference.Contains(text));
                    var remarksSpec = new DirectSpecification<ChequeBook>(c => c.Remarks.Contains(text));

                    specification &= (membershipNumberSpec | payrollNumbersSpec | identificationNumberSpec | addressLandLineSpec | addressMobileLineSpec | reference1Spec | reference2Spec | reference3Spec
                        | referenceSpec | remarksSpec);
                }
                else
                {
                    var nonIndividualSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                    var firstNameSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                    var lastNameSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                    var payrollNumbersSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                    var reference1Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                    var reference2Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                    var reference3Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                    var addressEmail = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                    var addressCity = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                    var addressPostalCode = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                    var addressAddressLine1 = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                    var addressAddressLine2 = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                    var addressStreet = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                    var addressLandLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                    var referenceSpec = new DirectSpecification<ChequeBook>(c => c.Reference.Contains(text));
                    var remarksSpec = new DirectSpecification<ChequeBook>(c => c.Remarks.Contains(text));

                    specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                    | referenceSpec | remarksSpec);
                }
            }

            return specification;
        }

        public static Specification<ChequeBook> ChequeBookFullText(string text, int type)
        {
            Specification<ChequeBook> specification = new DirectSpecification<ChequeBook>(x => x.Type == type);

            if (!String.IsNullOrWhiteSpace(text))
            {
                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var membershipNumberSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.SerialNumber == number);

                    var payrollNumbersSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));

                    var addressLandLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                    var reference1Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                    var reference2Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                    var reference3Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                    var referenceSpec = new DirectSpecification<ChequeBook>(c => c.Reference.Contains(text));
                    var remarksSpec = new DirectSpecification<ChequeBook>(c => c.Remarks.Contains(text));

                    specification &= (membershipNumberSpec | payrollNumbersSpec | identificationNumberSpec | addressLandLineSpec | addressMobileLineSpec | reference1Spec | reference2Spec | reference3Spec
                        | referenceSpec | remarksSpec);
                }
                else
                {
                    var nonIndividualSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                    var firstNameSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                    var lastNameSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                    var payrollNumbersSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                    var identificationNumberSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                    var reference1Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                    var reference2Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                    var reference3Spec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                    var addressEmail = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                    var addressCity = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                    var addressPostalCode = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                    var addressAddressLine1 = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                    var addressAddressLine2 = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                    var addressStreet = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                    var addressLandLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                    var addressMobileLineSpec = new DirectSpecification<ChequeBook>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                    var referenceSpec = new DirectSpecification<ChequeBook>(c => c.Reference.Contains(text));
                    var remarksSpec = new DirectSpecification<ChequeBook>(c => c.Remarks.Contains(text));

                    specification &= (nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                    | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec
                    | referenceSpec | remarksSpec);
                }
            }

            return specification;
        }
    }
}
