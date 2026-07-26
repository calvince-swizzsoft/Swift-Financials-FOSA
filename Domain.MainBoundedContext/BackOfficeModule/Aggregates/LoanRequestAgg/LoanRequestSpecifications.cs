using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;
using System.Globalization;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanRequestAgg
{

    public static class LoanRequestSpecifications
    {
        public static Specification<LoanRequest> DefaultSpec()
        {
            return new TrueSpecification<LoanRequest>();
        }

        public static Specification<LoanRequest> LoanRequestWithCustomerIdAndLoanProductId(Guid customerId, Guid loanProductId)
        {
            Specification<LoanRequest> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty && loanProductId != null && loanProductId != Guid.Empty)
            {
                var customerIdAndLoanProductIdSpec = new DirectSpecification<LoanRequest>(c => c.CustomerId == customerId && c.LoanProductId == loanProductId);

                specification &= customerIdAndLoanProductIdSpec;
            }

            return specification;
        }

        public static Specification<LoanRequest> LoanRequestWithStatus(int status)
        {
            Specification<LoanRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<LoanRequest>(x => x.Status == status);

            return specification;
        }

        public static Specification<LoanRequest> LoanRequestWithStatusAndFullText(int status, string text, int loanRequestFilter)
        {
            Specification<LoanRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<LoanRequest>(x => x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanRequestFilter)loanRequestFilter)
                {
                    case LoanRequestFilter.Reference:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanRequestFilter.Purpose:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanRequestFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanRequestFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanRequest>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanRequestFilter.CustomerSerialNumber:
                        break;
                    case LoanRequestFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanRequestFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanRequestFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanRequestFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanRequestFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanRequestFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanRequestFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanRequestFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanRequest> LoanRequestWithStatusAndFullText(DateTime startDate, DateTime endDate, int status, string text, int loanRequestFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<LoanRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanRequestFilter)loanRequestFilter)
                {
                    case LoanRequestFilter.Reference:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanRequestFilter.Purpose:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanRequestFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanRequestFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanRequest>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanRequestFilter.CustomerSerialNumber:
                        break;
                    case LoanRequestFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanRequestFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanRequestFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanRequestFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanRequestFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanRequestFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanRequestFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanRequestFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanRequest> LoanRequestFullText(string text, int loanRequestFilter)
        {
            Specification<LoanRequest> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanRequestFilter)loanRequestFilter)
                {
                    case LoanRequestFilter.Reference:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanRequestFilter.Purpose:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanRequestFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanRequestFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanRequest>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanRequestFilter.CustomerSerialNumber:
                        break;
                    case LoanRequestFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanRequestFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanRequestFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanRequestFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanRequestFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanRequestFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanRequestFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanRequestFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanRequest> LoanRequestWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text, int loanRequestFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<LoanRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<LoanRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((LoanRequestFilter)loanRequestFilter)
                {
                    case LoanRequestFilter.Reference:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    case LoanRequestFilter.Purpose:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanPurpose.Description) > 0);
                        break;
                    case LoanRequestFilter.LoanProduct:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.LoanProduct.Description) > 0);
                        break;
                    case LoanRequestFilter.AmountApplied:

                        var amountApplied = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amountApplied))
                        {
                            specification &= new DirectSpecification<LoanRequest>(x => x.AmountApplied == amountApplied);
                        }

                        break;
                    case LoanRequestFilter.CustomerSerialNumber:
                        break;
                    case LoanRequestFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerFirstName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case LoanRequestFilter.CustomerLastName:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case LoanRequestFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case LoanRequestFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case LoanRequestFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case LoanRequestFilter.CustomerStreet:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case LoanRequestFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case LoanRequestFilter.CustomerCity:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case LoanRequestFilter.CustomerEmail:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case LoanRequestFilter.CustomerLandLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference1:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference2:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case LoanRequestFilter.CustomerReference3:
                        specification &= new DirectSpecification<LoanRequest>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<LoanRequest> LoanRequestWithCustomerIdInProcess(Guid customerId)
        {
            Specification<LoanRequest> specification = new TrueSpecification<LoanRequest>();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<LoanRequest>(c => c.CustomerId == customerId && c.Status == (int)LoanRequestStatus.New);

                specification &= customerIdSpec;
            }

            return specification;
        }
    }
}
