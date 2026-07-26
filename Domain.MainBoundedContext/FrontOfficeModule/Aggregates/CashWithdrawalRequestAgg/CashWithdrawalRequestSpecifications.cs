using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashWithdrawalRequestAgg
{
    public static class CashWithdrawalRequestSpecifications
    {
        public static Specification<CashWithdrawalRequest> DefaultSpec()
        {
            Specification<CashWithdrawalRequest> specification = new TrueSpecification<CashWithdrawalRequest>();

            return specification;
        }

        public static Specification<CashWithdrawalRequest> CashWithdrawalRequestWithDateRange(DateTime startDate, DateTime endDate, int status)
        {
            Specification<CashWithdrawalRequest> specification = DefaultSpec();

            endDate = UberUtil.AdjustTimeSpan(endDate);

            specification &= new DirectSpecification<CashWithdrawalRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            return specification;
        }

        public static Specification<CashWithdrawalRequest> ActionableCashWithdrawalRequestWithCustomerAccountId(Guid customerAccountId, string createdBy, DateTime startDate, DateTime endDate)
        {
            Specification<CashWithdrawalRequest> specification = DefaultSpec();

            endDate = UberUtil.AdjustTimeSpan(endDate);

            specification &= new DirectSpecification<CashWithdrawalRequest>(x => x.CustomerAccountId == customerAccountId && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.CreatedBy == createdBy && (x.Status == (int)CashWithdrawalRequestAuthStatus.Pending || x.Status == (int)CashWithdrawalRequestAuthStatus.Authorized));

            return specification;
        }

        public static Specification<CashWithdrawalRequest> ActionableCashWithdrawalRequestWithChartOfAccountId(Guid chartOfAccountId, string createdBy, DateTime startDate, DateTime endDate)
        {
            Specification<CashWithdrawalRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<CashWithdrawalRequest>(x => x.ChartOfAccountId == chartOfAccountId && x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.CreatedBy == createdBy && (x.Status == (int)CashWithdrawalRequestAuthStatus.Pending || x.Status == (int)CashWithdrawalRequestAuthStatus.Authorized));

            return specification;
        }

        public static Specification<CashWithdrawalRequest> CashWithdrawalRequestWithCustomerAccountIdAndCreatedToday(Guid customerAccountId)
        {
            Specification<CashWithdrawalRequest> specification = DefaultSpec();

            specification &= new DirectSpecification<CashWithdrawalRequest>(x => x.CustomerAccountId == customerAccountId && x.CreatedDate.Year == DateTime.Today.Year && x.CreatedDate.Month == DateTime.Today.Month && x.CreatedDate.Day == DateTime.Today.Day);

            return specification;
        }

        public static Specification<CashWithdrawalRequest> CashWithdrawalRequestWithDateRangeAndFullText(DateTime startDate, DateTime endDate, int status, string text, int customerFilter)
        {
            Specification<CashWithdrawalRequest> specification = CashWithdrawalRequestWithDateRange(startDate, endDate, status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CashWithdrawalRequest>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<CashWithdrawalRequest>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
