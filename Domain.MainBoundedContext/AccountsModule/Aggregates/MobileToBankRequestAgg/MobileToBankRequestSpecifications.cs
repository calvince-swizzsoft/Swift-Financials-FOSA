using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.MobileToBankRequestAgg
{
    public static class MobileToBankRequestSpecifications
    {
        public static Specification<MobileToBankRequest> DefaultSpec()
        {
            Specification<MobileToBankRequest> specification = new TrueSpecification<MobileToBankRequest>();

            return specification;
        }

        public static Specification<MobileToBankRequest> MobileToBankRequestWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<MobileToBankRequest> specification = new DirectSpecification<MobileToBankRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var MSISDNSpec = new DirectSpecification<MobileToBankRequest>(c => c.MSISDN.Contains(text));
                var businessShortCodeSpec = new DirectSpecification<MobileToBankRequest>(c => c.BusinessShortCode.Contains(text));
                var invoiceNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.InvoiceNumber.Contains(text));
                var transIDSpec = new DirectSpecification<MobileToBankRequest>(c => c.TransID.Contains(text));
                var thirdPartyTransIDSpec = new DirectSpecification<MobileToBankRequest>(c => c.ThirdPartyTransID.Contains(text));
                var transTimeSpec = new DirectSpecification<MobileToBankRequest>(c => c.TransTime.Contains(text));
                var billRefNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.BillRefNumber.Contains(text));
                var kycInfoSpec = new DirectSpecification<MobileToBankRequest>(c => c.KYCInfo.Contains(text));

                var nonIndividualSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                specification &= (MSISDNSpec | businessShortCodeSpec | invoiceNumberSpec | transIDSpec | thirdPartyTransIDSpec | transTimeSpec | billRefNumberSpec | kycInfoSpec |
                    nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }

        public static Specification<MobileToBankRequest> MobileToBankRequestWithStatusAndRecordStatus(int status, int recordStatus, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<MobileToBankRequest> specification = new DirectSpecification<MobileToBankRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status && x.RecordStatus == recordStatus);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var MSISDNSpec = new DirectSpecification<MobileToBankRequest>(c => c.MSISDN.Contains(text));
                var businessShortCodeSpec = new DirectSpecification<MobileToBankRequest>(c => c.BusinessShortCode.Contains(text));
                var invoiceNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.InvoiceNumber.Contains(text));
                var transIDSpec = new DirectSpecification<MobileToBankRequest>(c => c.TransID.Contains(text));
                var thirdPartyTransIDSpec = new DirectSpecification<MobileToBankRequest>(c => c.ThirdPartyTransID.Contains(text));
                var transTimeSpec = new DirectSpecification<MobileToBankRequest>(c => c.TransTime.Contains(text));
                var billRefNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.BillRefNumber.Contains(text));
                var kycInfoSpec = new DirectSpecification<MobileToBankRequest>(c => c.KYCInfo.Contains(text));

                var nonIndividualSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                specification &= (MSISDNSpec | businessShortCodeSpec | invoiceNumberSpec | transIDSpec | thirdPartyTransIDSpec | transTimeSpec | billRefNumberSpec | kycInfoSpec |
                    nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }

        public static Specification<MobileToBankRequest> MobileToBankRequestWithDateRangeAndFilter(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<MobileToBankRequest> specification = new DirectSpecification<MobileToBankRequest>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var MSISDNSpec = new DirectSpecification<MobileToBankRequest>(c => c.MSISDN.Contains(text));
                var businessShortCodeSpec = new DirectSpecification<MobileToBankRequest>(c => c.BusinessShortCode.Contains(text));
                var invoiceNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.InvoiceNumber.Contains(text));
                var transIDSpec = new DirectSpecification<MobileToBankRequest>(c => c.TransID.Contains(text));
                var thirdPartyTransIDSpec = new DirectSpecification<MobileToBankRequest>(c => c.ThirdPartyTransID.Contains(text));
                var transTimeSpec = new DirectSpecification<MobileToBankRequest>(c => c.TransTime.Contains(text));
                var billRefNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.BillRefNumber.Contains(text));
                var kycInfoSpec = new DirectSpecification<MobileToBankRequest>(c => c.KYCInfo.Contains(text));

                var nonIndividualSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.NonIndividual.Description.Contains(text));

                var firstNameSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var reference1Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var reference2Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var reference3Spec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressEmail = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));
                var addressLandLineSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<MobileToBankRequest>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));

                specification &= (MSISDNSpec | businessShortCodeSpec | invoiceNumberSpec | transIDSpec | thirdPartyTransIDSpec | transTimeSpec | billRefNumberSpec | kycInfoSpec |
                    nonIndividualSpec | firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | reference1Spec | reference2Spec | reference3Spec
                | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet | addressLandLineSpec | addressMobileLineSpec);
            }

            return specification;
        }
    }
}