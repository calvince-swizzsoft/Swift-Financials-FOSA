using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchEntryAgg
{
    public static class CreditBatchEntrySpecifications
    {
        public static Specification<CreditBatchEntry> DefaultSpec()
        {
            Specification<CreditBatchEntry> specification = new TrueSpecification<CreditBatchEntry>();

            return specification;
        }

        public static Specification<CreditBatchEntry> CreditBatchEntryWithCreditBatchId(Guid creditBatchId, string text)
        {
            Specification<CreditBatchEntry> specification = DefaultSpec();

            if (creditBatchId != null && creditBatchId != Guid.Empty)
            {
                var creditBatchIdSpec = new DirectSpecification<CreditBatchEntry>(c => c.CreditBatchId == creditBatchId);

                specification &= creditBatchIdSpec;
            }

            return specification;
        }

        public static Specification<CreditBatchEntry> CreditBatchEntryWithCreditBatchId(Guid creditBatchId, string text, int creditBatchEntryFilter)
        {
            Specification<CreditBatchEntry> specification = new DirectSpecification<CreditBatchEntry>(c => c.CreditBatchId == creditBatchId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CreditBatchEntryFilter)creditBatchEntryFilter)
                {
                    case CreditBatchEntryFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CreditBatchEntry>(c => c.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CreditBatchEntryFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CreditBatchEntryFilter.FirstName:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CreditBatchEntryFilter.LastName:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CreditBatchEntryFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CreditBatchEntryFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CreditBatchEntryFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CreditBatchEntryFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CreditBatchEntryFilter.AddressLine1:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CreditBatchEntryFilter.AddressLine2:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CreditBatchEntryFilter.Street:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CreditBatchEntryFilter.PostalCode:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CreditBatchEntryFilter.City:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CreditBatchEntryFilter.Email:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CreditBatchEntryFilter.LandLine:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CreditBatchEntryFilter.MobileLine:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference1:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference2:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference3:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    case CreditBatchEntryFilter.Beneficiary:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.Beneficiary) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }

        public static Specification<CreditBatchEntry> PostedCreditBatchEntryWithCreditBatchId(Guid creditBatchId)
        {
            Specification<CreditBatchEntry> specification = new DirectSpecification<CreditBatchEntry>(x => x.CreditBatchId == creditBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }

        public static Specification<CreditBatchEntry> CreditBatchEntryWithCreditBatchTypeAndCustomerId(int creditBatchType, Guid customerId)
        {
            Specification<CreditBatchEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var creditBatchIdSpec = new DirectSpecification<CreditBatchEntry>(c => c.CreditBatch.Type == creditBatchType && c.CustomerAccount.CustomerId == customerId);

                specification &= creditBatchIdSpec;
            }

            return specification;
        }

        public static ISpecification<CreditBatchEntry> CreditBatchEntryWithDateRangeAndCreditTypeIdAndCustomerId(int creditBatchType, Guid customerId, int status, DateTime startDate, DateTime endDate, params Guid[] creditTypeIds)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CreditBatchEntry> specification = new DirectSpecification<CreditBatchEntry>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status && x.CustomerAccount.CustomerId == customerId && x.CreditBatch.Type == creditBatchType);

            var creditTypeSpecs = new List<Specification<CreditBatchEntry>>();

            if (creditTypeIds != null)
            {
                Array.ForEach(creditTypeIds, (creditTypeId) =>
                {
                    var creditTypeIdSpec = new DirectSpecification<CreditBatchEntry>(x => x.CreditBatch.CreditTypeId == creditTypeId);

                    creditTypeSpecs.Add(creditTypeIdSpec);
                });

                if (creditTypeSpecs.Any())
                {
                    var spec0 = creditTypeSpecs[0];

                    for (int i = 1; i < creditTypeSpecs.Count; i++)
                    {
                        spec0 |= creditTypeSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }

        public static Specification<CreditBatchEntry> QueableCreditBatchEntries(params int[] creditBatchTypes)
        {
            Specification<CreditBatchEntry> specification = new DirectSpecification<CreditBatchEntry>(c => c.Status == (int)BatchEntryStatus.Pending && c.CreditBatch.Status == (int)BatchStatus.Posted);

            var creditBatchTypeSpecs = new List<Specification<CreditBatchEntry>>();

            if (creditBatchTypes != null)
            {
                Array.ForEach(creditBatchTypes, (creditBatchType) =>
                {
                    var creditBatchTypeSpec = new DirectSpecification<CreditBatchEntry>(x => x.CreditBatch.Type == (int)creditBatchType);

                    creditBatchTypeSpecs.Add(creditBatchTypeSpec);
                });

                if (creditBatchTypeSpecs.Any())
                {
                    var spec0 = creditBatchTypeSpecs[0];

                    for (int i = 1; i < creditBatchTypeSpecs.Count; i++)
                    {
                        spec0 |= creditBatchTypeSpecs[i];
                    }

                    specification &= (spec0);
                }
            }

            return specification;
        }

        public static Specification<CreditBatchEntry> CreditBatchEntryWithDateRangeAndCreditBatchType(DateTime startDate, DateTime endDate, int creditBatchType, string text, int creditBatchEntryFilter)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<CreditBatchEntry> specification = new DirectSpecification<CreditBatchEntry>(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate && c.CreditBatch.Type == creditBatchType);

            if (!String.IsNullOrWhiteSpace(text))
            {
                text = text.SanitizePatIndexInput();

                switch ((CreditBatchEntryFilter)creditBatchEntryFilter)
                {
                    case CreditBatchEntryFilter.SerialNumber:

                        int number = default(int);

                        if (int.TryParse(text.StripPunctuation(), out number))
                        {
                            specification &= new DirectSpecification<CreditBatchEntry>(c => c.CustomerAccount.Customer.SerialNumber == number);
                        }

                        break;
                    case CreditBatchEntryFilter.PersonalIdentificationNumber:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.PersonalIdentificationNumber) > 0);
                        break;
                    case CreditBatchEntryFilter.FirstName:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.FirstName) > 0);
                        break;
                    case CreditBatchEntryFilter.LastName:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.LastName) > 0);
                        break;
                    case CreditBatchEntryFilter.IdentityCardNumber:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.IdentityCardNumber) > 0);
                        break;
                    case CreditBatchEntryFilter.PayrollNumbers:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Individual.PayrollNumbers) > 0);
                        break;
                    case CreditBatchEntryFilter.NonIndividual_Description:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.Description) > 0);
                        break;
                    case CreditBatchEntryFilter.NonIndividual_RegistrationNumber:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.NonIndividual.RegistrationNumber) > 0);
                        break;
                    case CreditBatchEntryFilter.AddressLine1:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine1) > 0);
                        break;
                    case CreditBatchEntryFilter.AddressLine2:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.AddressLine2) > 0);
                        break;
                    case CreditBatchEntryFilter.Street:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Street) > 0);
                        break;
                    case CreditBatchEntryFilter.PostalCode:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.PostalCode) > 0);
                        break;
                    case CreditBatchEntryFilter.City:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.City) > 0);
                        break;
                    case CreditBatchEntryFilter.Email:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.Email) > 0);
                        break;
                    case CreditBatchEntryFilter.LandLine:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.LandLine) > 0);
                        break;
                    case CreditBatchEntryFilter.MobileLine:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Address.MobileLine) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference1:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference1) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference2:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference2) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference3:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.CustomerAccount.Customer.Reference3) > 0);
                        break;
                    case CreditBatchEntryFilter.Beneficiary:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.Beneficiary) > 0);
                        break;
                    case CreditBatchEntryFilter.Reference:
                        specification &= new DirectSpecification<CreditBatchEntry>(c => SqlFunctions.PatIndex(text, c.Reference) > 0);
                        break;
                    default:
                        break;
                }
            }

            return specification;
        }
    }
}
