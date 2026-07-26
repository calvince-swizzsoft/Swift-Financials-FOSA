using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg
{
    public static class CashTransferRequestSpecifications
    {
        public static Specification<CashTransferRequest> DefaultSpec()
        {
            Specification<CashTransferRequest> specification = new TrueSpecification<CashTransferRequest>();

            return specification;
        }

        public static Specification<CashTransferRequest> CashTransferRequestWithDateRange(DateTime startDate, DateTime endDate, int status)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CashTransferRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<CashTransferRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            return specification;
        }

    
        public static Specification<CashTransferRequest> CashTransferRequestWithEmployeeId(Guid employeeId, DateTime startDate, DateTime endDate, int status)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CashTransferRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<CashTransferRequest>(x => x.EmployeeId == employeeId && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            return specification;
        }

        public static Specification<CashTransferRequest> ActionableCashTransferRequestWithEmployeeId(Guid employeeId)
        {
            Specification<CashTransferRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<CashTransferRequest>(x => x.EmployeeId == employeeId && (x.Status == (int)CashTransferRequestStatus.Acknowledged) && !x.Utilized);

            return specification;
        }

        public static Specification<CashTransferRequest> CashTransferRequestWithDateRangeStatusAndFullText(DateTime startDate, DateTime endDate, int status, string text, int customerFilter)
        {
            Specification<CashTransferRequest> specification = CashTransferRequestWithDateRange(startDate, endDate, status);

            specification &= new DirectSpecification<CashTransferRequest>(x => x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CashTransferRequest>(x => x.Employee.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<CashTransferRequest>(c => SqlFunctions.PatIndex(text, c.Employee.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
