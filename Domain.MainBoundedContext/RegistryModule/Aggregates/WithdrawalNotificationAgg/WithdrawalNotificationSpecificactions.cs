using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.WithdrawalNotificationAgg
{
    public static class WithdrawalNotificationSpecifications
    {
        public static Specification<WithdrawalNotification> DefaultSpec()
        {
            Specification<WithdrawalNotification> specification = new TrueSpecification<WithdrawalNotification>();

            return specification;
        }

        public static ISpecification<WithdrawalNotification> WithdrawalNotificationWithCustomerId(Guid customerId)
        {
            Specification<WithdrawalNotification> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<WithdrawalNotification>(x => x.CustomerId == customerId);
            }

            return specification;
        }

        public static Specification<WithdrawalNotification> WithdrawalNotificationFullText(string text, int customerFilter)
        {
            Specification<WithdrawalNotification> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<WithdrawalNotification>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<WithdrawalNotification> WithdrawalNotificationWithDateRange(DateTime startDate, DateTime endDate, int status)
        {
            Specification<WithdrawalNotification> specification = DefaultSpec();

            endDate = UberUtil.AdjustTimeSpan(endDate);

            specification &= new DirectSpecification<WithdrawalNotification>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            return specification;
        }

        public static Specification<WithdrawalNotification> WithdrawalNotificationWithDateRangeAndFullText(DateTime startDate, DateTime endDate, int status, string text, int customerFilter)
        {
            Specification<WithdrawalNotification> specification = WithdrawalNotificationWithDateRange(startDate, endDate, status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<WithdrawalNotification>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<WithdrawalNotification>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
