using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountCarryForwardInstallmentAgg
{
    public static class CustomerAccountCarryForwardInstallmentSpecifications
    {
        public static Specification<CustomerAccountCarryForwardInstallment> DefaultSpec()
        {
            Specification<CustomerAccountCarryForwardInstallment> specification = new TrueSpecification<CustomerAccountCarryForwardInstallment>();

            return specification;
        }

        public static ISpecification<CustomerAccountCarryForwardInstallment> CustomerAccountCarryForwardInstallmentWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<CustomerAccountCarryForwardInstallment> specification =
                new DirectSpecification<CustomerAccountCarryForwardInstallment>(x => x.CustomerAccountId == customerAccountId);

            return specification;
        }

        public static ISpecification<CustomerAccountCarryForwardInstallment> CustomerAccountCarryForwardInstallmentWithCustomerAccountIdAndChartOfAccountId(Guid customerAccountId, Guid chartOfAccountId)
        {
            Specification<CustomerAccountCarryForwardInstallment> specification =
                new DirectSpecification<CustomerAccountCarryForwardInstallment>(x => x.CustomerAccountId == customerAccountId && x.ChartOfAccountId == chartOfAccountId);

            return specification;
        }

        public static Specification<CustomerAccountCarryForwardInstallment> CustomerAccountCarryForwardInstallmentWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text, int customerFilter)
        {
            Specification<CustomerAccountCarryForwardInstallment> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<CustomerAccountCarryForwardInstallment>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
