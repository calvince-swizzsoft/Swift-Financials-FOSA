using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.AccountClosureRequestAgg
{
    public static class AccountClosureRequestSpecifications
    {
        public static Specification<AccountClosureRequest> DefaultSpec()
        {
            Specification<AccountClosureRequest> specification = new TrueSpecification<AccountClosureRequest>();

            return specification;
        }

        public static Specification<AccountClosureRequest> AccountClosureRequestFullText(string text, int customerFilter)
        {
            Specification<AccountClosureRequest> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<AccountClosureRequest>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<AccountClosureRequest> AccountClosureRequestWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<AccountClosureRequest> specification = DefaultSpec();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<AccountClosureRequest>(x => x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }

        public static Specification<AccountClosureRequest> AccountClosureRequestWithDateRange(DateTime startDate, DateTime endDate, int status)
        {
            Specification<AccountClosureRequest> specification = DefaultSpec();

            endDate = UberUtil.AdjustTimeSpan(endDate);

            specification &= new DirectSpecification<AccountClosureRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            return specification;
        }
        
        public static Specification<AccountClosureRequest> AccountClosureRequestWithDateRangeAndFullText(DateTime startDate, DateTime endDate, int status, string text, int customerFilter)
        {
            Specification<AccountClosureRequest> specification = AccountClosureRequestWithDateRange(startDate, endDate, status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<AccountClosureRequest>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<AccountClosureRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
