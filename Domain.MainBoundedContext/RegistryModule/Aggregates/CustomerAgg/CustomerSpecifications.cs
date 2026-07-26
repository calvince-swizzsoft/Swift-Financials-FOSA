using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg
{
    public static class CustomerSpecifications
    {
        public static Specification<Customer> DefaultSpec()
        {
            Specification<Customer> specification = new TrueSpecification<Customer>();

            return specification;
        }

        public static Specification<Customer> CustomerId(Guid customerId)
        {
            Specification<Customer> specification = new DirectSpecification<Customer>(x => x.Id == customerId);

            return specification;
        }

        public static Specification<Customer> CustomerFullText(string text, int customerFilter)
        {
            Specification<Customer> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<Customer>(x => x.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<Customer> CustomerType(int type, string text, int customerFilter)
        {
            Specification<Customer> specification = new DirectSpecification<Customer>(x => x.Type == type);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<Customer>(x => x.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<Customer> CustomerRecordStatus(int recordStatus, string text, int customerFilter)
        {
            Specification<Customer> specification = new DirectSpecification<Customer>(x => x.RecordStatus == recordStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<Customer>(x => x.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<Customer> CustomerSerialNumber(int serialNumber)
        {
            Specification<Customer> specification = new DirectSpecification<Customer>(c => c.SerialNumber == serialNumber);

            return specification;
        }

        public static Specification<Customer> CustomerPayrollNumbers(string payrollNumbers, bool matchExtact)
        {
            if (matchExtact)
            {
                return new DirectSpecification<Customer>(c => c.Individual.PayrollNumbers == payrollNumbers);
            }
            else
            {
                return new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(payrollNumbers, c.Individual.PayrollNumbers) > 0);
            }
        }

        public static Specification<Customer> CustomerIndividualIdentityCardNumber(string identityCardNumber, bool matchExtact)
        {
            if (matchExtact)
            {
                return new DirectSpecification<Customer>(c => c.Individual.IdentityCardNumber == identityCardNumber);
            }
            else
            {
                return new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(identityCardNumber, c.Individual.IdentityCardNumber) > 0);
            }
        }

        public static Specification<Customer> CustomerIndividualIDNumber(string identityCardNumber)
        {
            Specification<Customer> specification = DefaultSpec();

            var customerSpec = new DirectSpecification<Customer>(c => c.Individual.IdentityCardNumber == identityCardNumber);

            specification &= customerSpec;


            return specification;
        }

        public static Specification<Customer> CustomerNonIndividualRegistrationNumber(string registrationNumber)
        {
            Specification<Customer> specification = new DirectSpecification<Customer>(c => c.NonIndividual.RegistrationNumber == registrationNumber);

            return specification;
        }

        public static Specification<Customer> CustomerPersonalIdentificationNumber(string personalIdentificationNumber)
        {
            Specification<Customer> specification = new DirectSpecification<Customer>(c => c.PersonalIdentificationNumber == personalIdentificationNumber);

            return specification;
        }

        public static Specification<Customer> CustomerWithStationId(Guid stationId, string text, int customerFilter)
        {
            Specification<Customer> specification = DefaultSpec();

            specification &= new DirectSpecification<Customer>(c => c.StationId == stationId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<Customer>(x => x.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<Customer>(c => SqlFunctions.PatIndex(text, c.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}