using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FileRegisterAgg
{
    public static class FileRegisterSpecifications
    {
        public static Specification<FileRegister> DefaultSpec()
        {
            Specification<FileRegister> specification = new TrueSpecification<FileRegister>();

            return specification;
        }

        public static Specification<FileRegister> FileRegisterWithoutLastFileMovementDestinationDepartmentId(Guid lastFileMovementDestinationDepartmentId)
        {
            Specification<FileRegister> specification = DefaultSpec();

            if (lastFileMovementDestinationDepartmentId != null && lastFileMovementDestinationDepartmentId != Guid.Empty)
            {
                var saccoBranchIdSpec = new DirectSpecification<FileRegister>(c => (c.History.Any() && c.History.OrderByDescending(x => x.CreatedDate).FirstOrDefault().DestinationDepartmentId != lastFileMovementDestinationDepartmentId));

                specification &= saccoBranchIdSpec;
            }

            return specification;
        }

        public static Specification<FileRegister> FileRegisterWithStatusAndLastFileMovementDestinationDepartmentId(int status, Guid lastFileMovementDestinationDepartmentId, string text, int customerFilter)
        {
            Specification<FileRegister> specification = DefaultSpec();

            if (lastFileMovementDestinationDepartmentId != null && lastFileMovementDestinationDepartmentId != Guid.Empty)
            {
                var saccoBranchIdSpec = new DirectSpecification<FileRegister>(c => c.Status == status && (c.History.Any() && c.History.OrderByDescending(x => x.CreatedDate).FirstOrDefault().DestinationDepartmentId == lastFileMovementDestinationDepartmentId));

                specification &= saccoBranchIdSpec;
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
                            specification &= new DirectSpecification<FileRegister>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<FileRegister> FileRegisterWithCustomerId(Guid customerId)
        {
            Specification<FileRegister> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<FileRegister>(c => c.CustomerId == customerId);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<FileRegister> FileRegisterFullText(string text, int customerFilter)
        {
            Specification<FileRegister> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<FileRegister>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<FileRegister>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
