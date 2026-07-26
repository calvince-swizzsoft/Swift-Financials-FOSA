using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Data.Entity.SqlServer;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.AlternateChannelAgg
{
    public static class AlternateChannelSpecifications
    {
        public static Specification<AlternateChannel> DefaultSpec()
        {
            Specification<AlternateChannel> specification = new TrueSpecification<AlternateChannel>();

            return specification;
        }

        public static ISpecification<AlternateChannel> AlternateChannelWithCustomerId(Guid customerId)
        {
            Specification<AlternateChannel> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<AlternateChannel>(x => x.CustomerAccount.CustomerId == customerId);
            }

            return specification;
        }

        public static ISpecification<AlternateChannel> AlternateChannelWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<AlternateChannel> specification = DefaultSpec();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<AlternateChannel>(x => x.CustomerAccountId == customerAccountId);
            }

            return specification;
        }

        public static ISpecification<AlternateChannel> AlternateChannelWithCustomerAccountIdAndType(Guid customerAccountId, int type)
        {
            Specification<AlternateChannel> specification = DefaultSpec();

            if (customerAccountId != null && customerAccountId != Guid.Empty)
            {
                specification &= new DirectSpecification<AlternateChannel>(x => x.CustomerAccountId == customerAccountId && x.Type == type);
            }

            return specification;
        }

        public static ISpecification<AlternateChannel> AlternateChannelWithCardNumberAndType(string cardNumber, int type)
        {
            Specification<AlternateChannel> specification = DefaultSpec();

            specification &= new DirectSpecification<AlternateChannel>(c => c.Type == type && c.CardNumber == cardNumber);

            return specification;
        }

        public static ISpecification<AlternateChannel> AlternateChannelWithCardNumber(string cardNumber)
        {
            Specification<AlternateChannel> specification = DefaultSpec();

            specification &= new DirectSpecification<AlternateChannel>(c => c.CardNumber == cardNumber);

            return specification;
        }

        public static Specification<AlternateChannel> AlternateChannelFullText(string text, int alternateChannelFilter)
        {
            Specification<AlternateChannel> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((AlternateChannelFilter)alternateChannelFilter)
                {
                    case AlternateChannelFilter.PrimaryAccountNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CardNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<AlternateChannel>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case AlternateChannelFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerFirstName:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case AlternateChannelFilter.CustomerLastName:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case AlternateChannelFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case AlternateChannelFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case AlternateChannelFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case AlternateChannelFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case AlternateChannelFilter.CustomerStreet:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case AlternateChannelFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case AlternateChannelFilter.CustomerCity:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case AlternateChannelFilter.CustomerEmail:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case AlternateChannelFilter.CustomerLandLine:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case AlternateChannelFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference1:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference2:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference3:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<AlternateChannel> AlternateChannelWithTypeAndFullText(int type, int recordStatus, string text, int alternateChannelFilter)
        {
            Specification<AlternateChannel> specification = new DirectSpecification<AlternateChannel>(x => x.Type == type && x.RecordStatus == recordStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((AlternateChannelFilter)alternateChannelFilter)
                {
                    case AlternateChannelFilter.PrimaryAccountNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CardNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<AlternateChannel>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case AlternateChannelFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerFirstName:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case AlternateChannelFilter.CustomerLastName:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case AlternateChannelFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case AlternateChannelFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case AlternateChannelFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case AlternateChannelFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case AlternateChannelFilter.CustomerStreet:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case AlternateChannelFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case AlternateChannelFilter.CustomerCity:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case AlternateChannelFilter.CustomerEmail:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case AlternateChannelFilter.CustomerLandLine:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case AlternateChannelFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference1:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference2:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference3:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<AlternateChannel> ThirdPartyNotifiableAlternateChannels(int type, string text, int alternateChannelFilter)
        {
            Specification<AlternateChannel> specification = new DirectSpecification<AlternateChannel>(c => !c.IsLocked && c.Type == type && c.RecordStatus == (int)RecordStatus.Approved && !c.IsThirdPartyNotified);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((AlternateChannelFilter)alternateChannelFilter)
                {
                    case AlternateChannelFilter.PrimaryAccountNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CardNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<AlternateChannel>(x => x.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case AlternateChannelFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerFirstName:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case AlternateChannelFilter.CustomerLastName:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case AlternateChannelFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case AlternateChannelFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case AlternateChannelFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case AlternateChannelFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case AlternateChannelFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case AlternateChannelFilter.CustomerStreet:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case AlternateChannelFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case AlternateChannelFilter.CustomerCity:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case AlternateChannelFilter.CustomerEmail:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case AlternateChannelFilter.CustomerLandLine:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case AlternateChannelFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference1:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference2:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case AlternateChannelFilter.CustomerReference3:
                        specification &= new DirectSpecification<AlternateChannel>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
