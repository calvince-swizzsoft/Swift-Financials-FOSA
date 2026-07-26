using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.StandingOrderAgg
{
    public static class StandingOrderSpecifications
    {
        public static Specification<StandingOrder> DefaultSpec()
        {
            Specification<StandingOrder> specification = new TrueSpecification<StandingOrder>();

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountId(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty && beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId && x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId);
            }

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBenefactorCustomerAccountIdAndBeneficiaryCustomerAccountIdAndTrigger(Guid benefactorCustomerAccountId, Guid beneficiaryCustomerAccountId, int trigger)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty && beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId && x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId && x.Trigger == trigger);
            }

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBenefactorCustomerIdAndBenefactorCustomerAccountCustomerAccountTypeProductCode(Guid benefactorCustomerId, int customerAccountTypeProductCode)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (benefactorCustomerId != null && benefactorCustomerId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccount.CustomerId == benefactorCustomerId && x.BenefactorCustomerAccount.CustomerAccountType.ProductCode == customerAccountTypeProductCode);
            }

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBenefactorCustomerAccountId(Guid benefactorCustomerAccountId)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId);
            }

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBeneficiaryCustomerAccountId(Guid beneficiaryCustomerAccountId)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId);
            }

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBeneficiaryCustomerAccountIdAndTrigger(Guid beneficiaryCustomerAccountId, int trigger)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (beneficiaryCustomerAccountId != null && beneficiaryCustomerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BeneficiaryCustomerAccountId == beneficiaryCustomerAccountId && x.Trigger == trigger);
            }

            return specification;
        }

        public static ISpecification<StandingOrder> StandingOrderWithBenefactorCustomerAccountIdAndTrigger(Guid benefactorCustomerAccountId, int trigger)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            if (benefactorCustomerAccountId != null && benefactorCustomerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccountId == benefactorCustomerAccountId && x.Trigger == trigger);
            }

            return specification;
        }

        public static Specification<StandingOrder> StandingOrderFullText(string text, int customerAccountFilter, int customerFilter)
        {
            Specification<StandingOrder> specification = new TrueSpecification<StandingOrder>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((StandingOrderCustomerAccountFilter)customerAccountFilter)
                {
                    case StandingOrderCustomerAccountFilter.Beneficiary:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BeneficiaryCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    case StandingOrderCustomerAccountFilter.Benefactor:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<StandingOrder> StandingOrderFullText(int trigger, string text, int customerAccountFilter, int customerFilter)
        {
            Specification<StandingOrder> specification = DefaultSpec();

            var triggerSpec = new DirectSpecification<StandingOrder>(x => x.Trigger == trigger);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((StandingOrderCustomerAccountFilter)customerAccountFilter)
                {
                    case StandingOrderCustomerAccountFilter.Beneficiary:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BeneficiaryCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    case StandingOrderCustomerAccountFilter.Benefactor:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }

                specification &= triggerSpec;
            }
            else specification &= triggerSpec;

            return specification;
        }

        public static Specification<StandingOrder> DueStandingOrders(DateTime targetDate, int targetDateOption, string text, int customerAccountFilter, int customerFilter)
        {
            Specification<StandingOrder> specification = new DirectSpecification<StandingOrder>(c => c.Trigger == (int)StandingOrderTrigger.Schedule && c.Duration.EndDate >= targetDate && !c.IsLocked);

            switch (targetDateOption)
            {
                case 1:
                    specification &= new DirectSpecification<StandingOrder>(c => DateTime.Compare(c.Schedule.ExpectedRunDate, targetDate) == 0);
                    break;
                case 0:
                default:
                    specification &= new DirectSpecification<StandingOrder>(c => DateTime.Compare(c.Schedule.ActualRunDate, targetDate) == 0);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((StandingOrderCustomerAccountFilter)customerAccountFilter)
                {
                    case StandingOrderCustomerAccountFilter.Beneficiary:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BeneficiaryCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    case StandingOrderCustomerAccountFilter.Benefactor:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }
            }

            specification |= new DirectSpecification<StandingOrder>(c => !c.IsLocked && c.Trigger == (int)StandingOrderTrigger.Schedule && c.Schedule.ForceExecute && c.Duration.EndDate >= targetDate);

            return specification;
        }

        public static Specification<StandingOrder> SkippedStandingOrders(DateTime targetDate, string text, int customerAccountFilter, int customerFilter)
        {
            Specification<StandingOrder> specification = new DirectSpecification<StandingOrder>(c => !c.IsLocked && c.Trigger == (int)StandingOrderTrigger.Schedule && c.Schedule.ActualRunDate < targetDate && c.Duration.EndDate > targetDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((StandingOrderCustomerAccountFilter)customerAccountFilter)
                {
                    case StandingOrderCustomerAccountFilter.Beneficiary:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BeneficiaryCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BeneficiaryCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    case StandingOrderCustomerAccountFilter.Benefactor:

                        switch ((CustomerFilter)customerFilter)
                        {
                            case CustomerFilter.SerialNumber:

                                int number = default(int);

                                if (int.TryParse(text.StripPunctuation(), out number))
                                {
                                    specification &= new DirectSpecification<StandingOrder>(x => x.BenefactorCustomerAccount.Customer.SerialNumber == number);
                                }

                                break;
                            case CustomerFilter.PersonalIdentificationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                                break;
                            case CustomerFilter.FirstName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.FirstName) > 0);
                                break;
                            case CustomerFilter.LastName:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.LastName) > 0);
                                break;
                            case CustomerFilter.IdentityCardNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                                break;
                            case CustomerFilter.PayrollNumbers:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                                break;
                            case CustomerFilter.NonIndividual_Description:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.Description) > 0);
                                break;
                            case CustomerFilter.NonIndividual_RegistrationNumber:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                                break;
                            case CustomerFilter.AddressLine1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine1) > 0);
                                break;
                            case CustomerFilter.AddressLine2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.AddressLine2) > 0);
                                break;
                            case CustomerFilter.Street:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Street) > 0);
                                break;
                            case CustomerFilter.PostalCode:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.PostalCode) > 0);
                                break;
                            case CustomerFilter.City:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.City) > 0);
                                break;
                            case CustomerFilter.Email:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.Email) > 0);
                                break;
                            case CustomerFilter.LandLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.LandLine) > 0);
                                break;
                            case CustomerFilter.MobileLine:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Address.MobileLine) > 0);
                                break;
                            case CustomerFilter.Reference1:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference1) > 0);
                                break;
                            case CustomerFilter.Reference2:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference2) > 0);
                                break;
                            case CustomerFilter.Reference3:
                                specification &= new DirectSpecification<StandingOrder>(c => SqlFunctions.PatIndex(text, c.BenefactorCustomerAccount.Customer.Reference3) > 0);
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }
            }

            return specification;
        }


        public static Specification<StandingOrder> ArrearsStandingOrders(params int[] standingOrderTriggers)
        {
            Specification<StandingOrder> specification = new DirectSpecification<StandingOrder>(c => !c.IsLocked && (c.Principal + c.Interest) > 0m);

            var standingOrderTriggerSpecs = new List<Specification<StandingOrder>>();

            if (standingOrderTriggers != null)
            {
                Array.ForEach(standingOrderTriggers, (trigger) =>
                {
                    var standingOrderTriggerSpec = new DirectSpecification<StandingOrder>(x => x.Trigger == trigger);

                    standingOrderTriggerSpecs.Add(standingOrderTriggerSpec);
                });

                if (standingOrderTriggerSpecs.Any())
                {
                    var spec0 = standingOrderTriggerSpecs[0];

                    for (int i = 1; i < standingOrderTriggerSpecs.Count; i++)
                    {
                        spec0 |= standingOrderTriggerSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }
    }
}
