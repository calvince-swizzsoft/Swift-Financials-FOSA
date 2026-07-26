using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg
{
    public static class CustomerAccountSpecifications
    {
        public static Specification<CustomerAccount> DefaultSpec()
        {
            Specification<CustomerAccount> specification = new TrueSpecification<CustomerAccount>();

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerId(Guid customerId)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<CustomerAccount>(c => c.CustomerId == customerId);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductId(Guid customerId, params Guid[] accountTypeTargetProductIds)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerAccount>(c => c.CustomerId == customerId);

                var accountTypeTargetProductIdSpecs = new List<Specification<CustomerAccount>>();

                if (accountTypeTargetProductIds != null)
                {
                    Array.ForEach(accountTypeTargetProductIds, (accountTypeTargetProductId) =>
                    {
                        var accountTypeTargetProductIdSpec = new DirectSpecification<CustomerAccount>(x => x.CustomerAccountType.TargetProductId == accountTypeTargetProductId);

                        accountTypeTargetProductIdSpecs.Add(accountTypeTargetProductIdSpec);
                    });

                    if (accountTypeTargetProductIdSpecs.Any())
                    {
                        var spec0 = accountTypeTargetProductIdSpecs[0];

                        for (int i = 1; i < accountTypeTargetProductIdSpecs.Count; i++)
                        {
                            spec0 |= accountTypeTargetProductIdSpecs[i];
                        }

                        specification &= (spec0);
                    }
                }
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerIdAndCustomerAccountTypeTargetProductCode(Guid customerId, params int[] productCodes)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                specification &= new DirectSpecification<CustomerAccount>(c => c.CustomerId == customerId);

                var productCodeSpecs = new List<Specification<CustomerAccount>>();

                if (productCodes != null)
                {
                    Array.ForEach(productCodes, (productCode) =>
                    {
                        var accountTypeTargetProductCodeSpec = new DirectSpecification<CustomerAccount>(x => x.CustomerAccountType.ProductCode == productCode);

                        productCodeSpecs.Add(accountTypeTargetProductCodeSpec);
                    });

                    if (productCodeSpecs.Any())
                    {
                        var spec0 = productCodeSpecs[0];

                        for (int i = 1; i < productCodeSpecs.Count; i++)
                        {
                            spec0 |= productCodeSpecs[i];
                        }

                        specification &= (spec0);
                    }
                }
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerAccountTypeTargetProductId(Guid targetProductId)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (targetProductId != null && targetProductId != Guid.Empty)
            {
                var targetProductIdSpec = new DirectSpecification<CustomerAccount>(c => c.CustomerAccountType.TargetProductId == targetProductId);

                specification &= targetProductIdSpec;
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerAccountTypeTargetProductIdAndCustomerEmployerId(Guid targetProductId, Guid customerEmployerId)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (targetProductId != null && targetProductId != Guid.Empty && customerEmployerId != null && customerEmployerId != Guid.Empty)
            {
                var targetProductIdSpec = new DirectSpecification<CustomerAccount>(c => c.CustomerAccountType.TargetProductId == targetProductId);

                specification &= targetProductIdSpec;
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountFullText(string text, int customerFilter)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CustomerAccount>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerAccountTypeProductCodeAndFullText(int productCode, string text, int customerFilter)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            specification &= new DirectSpecification<CustomerAccount>(c => c.CustomerAccountType.ProductCode == productCode);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CustomerAccount>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountWithCustomerAccountTypeProductCodeAndRecordStatusAndFullText(int productCode, int recordStatus, string text, int customerFilter)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            specification &= new DirectSpecification<CustomerAccount>(c => c.CustomerAccountType.ProductCode == productCode && c.RecordStatus == recordStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CustomerFilter)customerFilter)
                {
                    case CustomerFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CustomerAccount>(x => x.Customer.SerialNumber == number);
                        }

                        break;
                    case CustomerFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CustomerFilter.FirstName:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.FirstName) > 0);
                        break;
                    case CustomerFilter.LastName:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.LastName) > 0);
                        break;
                    case CustomerFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CustomerFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CustomerFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.Description) > 0);
                        break;
                    case CustomerFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CustomerFilter.AddressLine1:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine1) > 0);
                        break;
                    case CustomerFilter.AddressLine2:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.AddressLine2) > 0);
                        break;
                    case CustomerFilter.Street:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Street) > 0);
                        break;
                    case CustomerFilter.PostalCode:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.PostalCode) > 0);
                        break;
                    case CustomerFilter.City:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.City) > 0);
                        break;
                    case CustomerFilter.Email:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.Email) > 0);
                        break;
                    case CustomerFilter.LandLine:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.LandLine) > 0);
                        break;
                    case CustomerFilter.MobileLine:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Address.MobileLine) > 0);
                        break;
                    case CustomerFilter.Reference1:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference1) > 0);
                        break;
                    case CustomerFilter.Reference2:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference2) > 0);
                        break;
                    case CustomerFilter.Reference3:
                        specification &= new DirectSpecification<CustomerAccount>(c => SqlFunctions.PatIndex(text, c.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<CustomerAccount> CustomerAccountFullAccountNumber(string text)
        {
            Specification<CustomerAccount> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var buffer = text.Split(new char[] { '-' });

                if (buffer.Length == 4)
                {
                    int branchCode = -1;

                    int serialNumber = -1;

                    int productCode = -1;

                    int targetProductCode = -1;

                    if (int.TryParse(buffer[0], out branchCode) && int.TryParse(buffer[1], out serialNumber) && int.TryParse(buffer[2], out productCode) && int.TryParse(buffer[3], out targetProductCode))
                    {
                        specification &= new DirectSpecification<CustomerAccount>(c => c.Customer.SerialNumber == serialNumber && c.CustomerAccountType.TargetProductCode == targetProductCode && c.CustomerAccountType.ProductCode == productCode && c.Branch.Code == branchCode);
                    }
                }
            }

            return specification;
        }

        public static ISpecification<CustomerAccount> CustomerAccountWithId(params Guid[] customerAccountIds)
        {
            Specification<CustomerAccount> specification = new TrueSpecification<CustomerAccount>();

            var customerAccountIdSpecs = new List<Specification<CustomerAccount>>();

            if (customerAccountIds != null)
            {
                Array.ForEach(customerAccountIds, (customerAccountIdId) =>
                {
                    var alternateChannelLogIdSpec = new DirectSpecification<CustomerAccount>(x => x.Id == customerAccountIdId);

                    customerAccountIdSpecs.Add(alternateChannelLogIdSpec);
                });

                if (customerAccountIdSpecs.Any())
                {
                    var spec0 = customerAccountIdSpecs[0];

                    for (int i = 1; i < customerAccountIdSpecs.Count; i++)
                    {
                        spec0 |= customerAccountIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }
    }
}
