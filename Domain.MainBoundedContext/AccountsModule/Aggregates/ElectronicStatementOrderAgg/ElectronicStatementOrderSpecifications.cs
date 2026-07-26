using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ElectronicStatementOrderAgg
{
    public static class ElectronicStatementOrderSpecifications
    {
        public static Specification<ElectronicStatementOrder> DefaultSpec()
        {
            Specification<ElectronicStatementOrder> specification = new TrueSpecification<ElectronicStatementOrder>();

            return specification;
        }

        public static ISpecification<ElectronicStatementOrder> ElectronicStatementOrderWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<ElectronicStatementOrder> specification = DefaultSpec();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<ElectronicStatementOrder>(x => x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }

        public static ISpecification<ElectronicStatementOrder> ElectronicStatementOrderWithCustomerIdAndCustomerAccountCustomerAccountTypeProductCode(Guid customerId, int customerAccountTypeProductCode)
        {
            Specification<ElectronicStatementOrder> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<ElectronicStatementOrder>(x => x.CustomerAccount.CustomerId == customerId && x.CustomerAccount.CustomerAccountType.ProductCode == customerAccountTypeProductCode);
            }

            return specification;
        }

        public static Specification<ElectronicStatementOrder> ElectronicStatementOrderFullText(string text, int customerFilter)
        {
            Specification<ElectronicStatementOrder> specification = new TrueSpecification<ElectronicStatementOrder>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<ElectronicStatementOrder>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<ElectronicStatementOrder> DueElectronicStatementOrders(DateTime targetDate, int targetDateOption, string text, int customerFilter)
        {
            Specification<ElectronicStatementOrder> specification = new DirectSpecification<ElectronicStatementOrder>(c => !c.IsLocked && c.Duration.EndDate >= targetDate);

            switch (targetDateOption)
            {
                case 1:
                    specification &= new DirectSpecification<ElectronicStatementOrder>(c => DateTime.Compare(c.Schedule.ExpectedRunDate, targetDate) == 0);
                    break;
                case 0:
                default:
                    specification &= new DirectSpecification<ElectronicStatementOrder>(c => DateTime.Compare(c.Schedule.ActualRunDate, targetDate) == 0);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<ElectronicStatementOrder>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<ElectronicStatementOrder> SkippedElectronicStatementOrders(DateTime targetDate, string text, int customerFilter)
        {
            Specification<ElectronicStatementOrder> specification = DefaultSpec();

            specification &= new DirectSpecification<ElectronicStatementOrder>(c => !c.IsLocked);

            specification &= new DirectSpecification<ElectronicStatementOrder>(s => s.Schedule.ActualRunDate < targetDate && s.Duration.EndDate > targetDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<ElectronicStatementOrder>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<ElectronicStatementOrder>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
