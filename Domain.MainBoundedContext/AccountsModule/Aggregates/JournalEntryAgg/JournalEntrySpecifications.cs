using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalEntryAgg
{
    public static class JournalEntrySpecifications
    {
        public static Specification<JournalEntry> JournalEntriesWithJournalIds(params Guid[] journalIds)
        {
            Specification<JournalEntry> specification = new TrueSpecification<JournalEntry>();

            var journalIdSpecs = new List<Specification<JournalEntry>>();

            if (journalIds != null)
            {
                Array.ForEach(journalIds, (journalId) =>
                {
                    var jounrnalIdSpec = new DirectSpecification<JournalEntry>(x => x.JournalId == journalId);

                    journalIdSpecs.Add(jounrnalIdSpec);
                });

                if (journalIdSpecs.Any())
                {
                    var spec0 = journalIdSpecs[0];

                    for (int i = 1; i < journalIdSpecs.Count; i++)
                    {
                        spec0 |= journalIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }

        public static ISpecification<JournalEntry> JournalEntriesWithDateRange(DateTime startDate, DateTime endDate, string text, int journalEntryFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<JournalEntry> specification = new DirectSpecification<JournalEntry>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((JournalEntryFilter)journalEntryFilter)
                {
                    case JournalEntryFilter.JournalPostingPeriod:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PostingPeriod.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalBranch:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Branch.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalPrimaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PrimaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalSecondaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.SecondaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalReference:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Reference) > 0);
                        break;
                    case JournalEntryFilter.JournalApplicationUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.ApplicationUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMachineName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMachineName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentDomainName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentDomainName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentOSVersion:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentOSVersion) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMACAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMACAddress) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMotherboardSerialNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMotherboardSerialNumber) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentProcessorId:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentProcessorId) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentIPAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentIPAddress) > 0);
                        break;
                    case JournalEntryFilter.ChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.ContraChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ContraChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.Amount:

                        var amount = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amount))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.Amount == amount);
                        }

                        break;
                    case JournalEntryFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case JournalEntryFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerFirstName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case JournalEntryFilter.CustomerLastName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case JournalEntryFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case JournalEntryFilter.CustomerStreet:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case JournalEntryFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case JournalEntryFilter.CustomerCity:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case JournalEntryFilter.CustomerEmail:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case JournalEntryFilter.CustomerLandLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference3:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }


            return specification;
        }

        public static ISpecification<JournalEntry> JournalEntriesWithCustomerAccountIdAndDateRange(Guid customerAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, params Guid[] chartOfAccountIds)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<JournalEntry> specification = new DirectSpecification<JournalEntry>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            var chartOfAccountIdSpecs = new List<Specification<JournalEntry>>();

            if (chartOfAccountIds != null)
            {
                Array.ForEach(chartOfAccountIds, (chartOfAccountId) =>
                {
                    var chartOfAccountIdSpec = new DirectSpecification<JournalEntry>(x => x.ChartOfAccountId == chartOfAccountId);

                    chartOfAccountIdSpecs.Add(chartOfAccountIdSpec);
                });

                if (chartOfAccountIdSpecs.Any())
                {
                    var spec0 = chartOfAccountIdSpecs[0];

                    for (int i = 1; i < chartOfAccountIdSpecs.Count; i++)
                    {
                        spec0 |= chartOfAccountIdSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            specification &= new DirectSpecification<JournalEntry>(x => x.CustomerAccountId == customerAccountId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((JournalEntryFilter)journalEntryFilter)
                {
                    case JournalEntryFilter.JournalPostingPeriod:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PostingPeriod.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalBranch:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Branch.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalPrimaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PrimaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalSecondaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.SecondaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalReference:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Reference) > 0);
                        break;
                    case JournalEntryFilter.JournalApplicationUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.ApplicationUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMachineName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMachineName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentDomainName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentDomainName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentOSVersion:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentOSVersion) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMACAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMACAddress) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMotherboardSerialNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMotherboardSerialNumber) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentProcessorId:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentProcessorId) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentIPAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentIPAddress) > 0);
                        break;
                    case JournalEntryFilter.ChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.ContraChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ContraChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.Amount:

                        var amount = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amount))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.Amount == amount);
                        }

                        break;
                    case JournalEntryFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case JournalEntryFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerFirstName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case JournalEntryFilter.CustomerLastName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case JournalEntryFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case JournalEntryFilter.CustomerStreet:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case JournalEntryFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case JournalEntryFilter.CustomerCity:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case JournalEntryFilter.CustomerEmail:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case JournalEntryFilter.CustomerLandLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference3:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }


            return specification;
        }

        public static ISpecification<JournalEntry> JournalEntriesWithChartOfAccountIdAndDateRange(Guid chartOfAccountId, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, int transactionDateFilter)
        {
            Specification<JournalEntry> specification = null;

            endDate = UberUtil.AdjustTimeSpan(endDate);

            switch ((TransactionDateFilter)transactionDateFilter)
            {
                case TransactionDateFilter.ValueDate:
                    specification = new DirectSpecification<JournalEntry>(x => x.ValueDate >= startDate && x.ValueDate <= endDate && x.ChartOfAccountId == chartOfAccountId);
                    break;
                case TransactionDateFilter.CreatedDate:
                    specification = new DirectSpecification<JournalEntry>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.ChartOfAccountId == chartOfAccountId);
                    break;
                default:
                    break;
            }

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((JournalEntryFilter)journalEntryFilter)
                {
                    case JournalEntryFilter.JournalPostingPeriod:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PostingPeriod.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalBranch:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Branch.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalPrimaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PrimaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalSecondaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.SecondaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalReference:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Reference) > 0);
                        break;
                    case JournalEntryFilter.JournalApplicationUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.ApplicationUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMachineName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMachineName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentDomainName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentDomainName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentOSVersion:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentOSVersion) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMACAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMACAddress) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMotherboardSerialNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMotherboardSerialNumber) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentProcessorId:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentProcessorId) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentIPAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentIPAddress) > 0);
                        break;
                    case JournalEntryFilter.ChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.ContraChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ContraChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.Amount:

                        var amount = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amount))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.Amount == amount);
                        }

                        break;
                    case JournalEntryFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case JournalEntryFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerFirstName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case JournalEntryFilter.CustomerLastName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case JournalEntryFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case JournalEntryFilter.CustomerStreet:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case JournalEntryFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case JournalEntryFilter.CustomerCity:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case JournalEntryFilter.CustomerEmail:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case JournalEntryFilter.CustomerLandLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference3:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }
            
            return specification;
        }

        public static ISpecification<JournalEntry> JournalEntriesWithChartOfAccountIdAndDateRange(Guid chartOfAccountId, DateTime startDate, DateTime endDate, int journalTransactionCode, string journalReference, int transactionDateFilter)
        {
            Specification<JournalEntry> specification = null;

            endDate = UberUtil.AdjustTimeSpan(endDate);

            switch ((TransactionDateFilter)transactionDateFilter)
            {
                case TransactionDateFilter.ValueDate:
                    specification = new DirectSpecification<JournalEntry>(x => x.ValueDate >= startDate && x.ValueDate <= endDate && x.ChartOfAccountId == chartOfAccountId);
                    break;
                case TransactionDateFilter.CreatedDate:
                    specification = new DirectSpecification<JournalEntry>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.ChartOfAccountId == chartOfAccountId);
                    break;
                default:
                    break;
            }

            specification &= new DirectSpecification<JournalEntry>(x => x.Journal.TransactionCode == journalTransactionCode);

            specification &= new DirectSpecification<JournalEntry>(x => x.Journal.Reference == journalReference);
            
            return specification;
        }

        public static ISpecification<JournalEntry> ReversibleJournalEntriesWithDateRange(int systemTransactionCode, DateTime startDate, DateTime endDate, string text, int journalEntryFilter, ServiceHeader serviceHeader)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<JournalEntry> specification = new DirectSpecification<JournalEntry>(x =>
                x.CreatedDate >= startDate && x.CreatedDate <= endDate &&
                x.Journal.TransactionCode == systemTransactionCode &&
                !x.Journal.IsLocked /*locked entries cannot be reversed*/ &&
                x.Journal.ApplicationUserName != serviceHeader.ApplicationUserName /*user cannot reverse own tx*/);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((JournalEntryFilter)journalEntryFilter)
                {
                    case JournalEntryFilter.JournalPostingPeriod:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PostingPeriod.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalBranch:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Branch.Description) > 0);
                        break;
                    case JournalEntryFilter.JournalPrimaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.PrimaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalSecondaryDescription:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.SecondaryDescription) > 0);
                        break;
                    case JournalEntryFilter.JournalReference:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.Reference) > 0);
                        break;
                    case JournalEntryFilter.JournalApplicationUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.ApplicationUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentUserName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentUserName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMachineName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMachineName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentDomainName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentDomainName) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentOSVersion:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentOSVersion) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMACAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMACAddress) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentMotherboardSerialNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentMotherboardSerialNumber) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentProcessorId:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentProcessorId) > 0);
                        break;
                    case JournalEntryFilter.JournalEnvironmentIPAddress:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.Journal.EnvironmentIPAddress) > 0);
                        break;
                    case JournalEntryFilter.ChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.ContraChartOfAccount:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.ContraChartOfAccount.AccountName) > 0);
                        break;
                    case JournalEntryFilter.Amount:

                        var amount = 0m;

                        if (decimal.TryParse(text.StripPunctuation(), NumberStyles.Any, CultureInfo.CurrentCulture, out amount))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.Amount == amount);
                        }

                        break;
                    case JournalEntryFilter.CustomerSerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<JournalEntry>(c => c.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case JournalEntryFilter.CustomerPersonalIdentificationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerFirstName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case JournalEntryFilter.CustomerLastName:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case JournalEntryFilter.CustomerIdentityCardNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerPayrollNumbers:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_Description:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case JournalEntryFilter.CustomerNonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case JournalEntryFilter.CustomerAddressLine2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case JournalEntryFilter.CustomerStreet:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case JournalEntryFilter.CustomerPostalCode:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case JournalEntryFilter.CustomerCity:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case JournalEntryFilter.CustomerEmail:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case JournalEntryFilter.CustomerLandLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerMobileLine:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference1:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference2:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case JournalEntryFilter.CustomerReference3:
                        specification &= new DirectSpecification<JournalEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
