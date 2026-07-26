using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg
{
    public static class ExternalChequeSpecifications
    {
        public static Specification<ExternalCheque> DefaultSpec()
        {
            Specification<ExternalCheque> specification = new TrueSpecification<ExternalCheque>();

            return specification;
        }

        public static Specification<ExternalCheque> UnClearedExternalChequesWithCustomerAccountId(Guid customerAccountId)
        {
            Specification<ExternalCheque> specification = new DirectSpecification<ExternalCheque>(x => x.CustomerAccountId == customerAccountId && !x.IsCleared && x.ClearedDate == null);

            return specification;
        }

        public static Specification<ExternalCheque> UnClearedExternalCheques(string text)
        {
            Specification<ExternalCheque> specification = new DirectSpecification<ExternalCheque>(x => !x.IsCleared && x.ClearedDate == null);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var tellerSpec = new DirectSpecification<ExternalCheque>(c => c.Teller.Description.Contains(text));
                var numberSpec = new DirectSpecification<ExternalCheque>(c => c.Number.Contains(text));
                var drawerSpec = new DirectSpecification<ExternalCheque>(c => c.Drawer.Contains(text));
                var drawerBankSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBank.Contains(text));
                var drawerBankBranchSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBankBranch.Contains(text));
                var chequeTypeSpec = new DirectSpecification<ExternalCheque>(c => c.ChequeType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<ExternalCheque>(c => c.CreatedBy.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (tellerSpec | numberSpec | drawerSpec | drawerBankSpec | drawerBankBranchSpec | chequeTypeSpec | createdBySpec |
                    payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 | addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static Specification<ExternalCheque> UnTransferredExternalChequesWithTellerId(Guid tellerId, string text)
        {
            Specification<ExternalCheque> specification = new DirectSpecification<ExternalCheque>(x => x.TellerId == tellerId && !x.IsTransferred && x.TransferredDate == null);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var tellerSpec = new DirectSpecification<ExternalCheque>(c => c.Teller.Description.Contains(text));
                var numberSpec = new DirectSpecification<ExternalCheque>(c => c.Number.Contains(text));
                var drawerSpec = new DirectSpecification<ExternalCheque>(c => c.Drawer.Contains(text));
                var drawerBankSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBank.Contains(text));
                var drawerBankBranchSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBankBranch.Contains(text));
                var chequeTypeSpec = new DirectSpecification<ExternalCheque>(c => c.ChequeType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<ExternalCheque>(c => c.CreatedBy.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (tellerSpec | numberSpec | drawerSpec | drawerBankSpec | drawerBankBranchSpec | chequeTypeSpec | createdBySpec |
                    payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 | addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static Specification<ExternalCheque> UnBankedExternalCheques(string text)
        {
            Specification<ExternalCheque> specification = new DirectSpecification<ExternalCheque>(x => x.IsTransferred && !x.IsBanked && x.BankedDate == null);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var tellerSpec = new DirectSpecification<ExternalCheque>(c => c.Teller.Description.Contains(text));
                var numberSpec = new DirectSpecification<ExternalCheque>(c => c.Number.Contains(text));
                var drawerSpec = new DirectSpecification<ExternalCheque>(c => c.Drawer.Contains(text));
                var drawerBankSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBank.Contains(text));
                var drawerBankBranchSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBankBranch.Contains(text));
                var chequeTypeSpec = new DirectSpecification<ExternalCheque>(c => c.ChequeType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<ExternalCheque>(c => c.CreatedBy.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (tellerSpec | numberSpec | drawerSpec | drawerBankSpec | drawerBankBranchSpec | chequeTypeSpec | createdBySpec |
                    payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 | addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static ISpecification<ExternalCheque> UnClearedExternalChequeWithMaturityDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<ExternalCheque> specification = new DirectSpecification<ExternalCheque>(x => !x.IsCleared && x.ClearedDate == null && x.MaturityDate >= startDate && x.MaturityDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var tellerSpec = new DirectSpecification<ExternalCheque>(c => c.Teller.Description.Contains(text));
                var numberSpec = new DirectSpecification<ExternalCheque>(c => c.Number.Contains(text));
                var drawerSpec = new DirectSpecification<ExternalCheque>(c => c.Drawer.Contains(text));
                var drawerBankSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBank.Contains(text));
                var drawerBankBranchSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBankBranch.Contains(text));
                var chequeTypeSpec = new DirectSpecification<ExternalCheque>(c => c.ChequeType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<ExternalCheque>(c => c.CreatedBy.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (tellerSpec | numberSpec | drawerSpec | drawerBankSpec | drawerBankBranchSpec | chequeTypeSpec | createdBySpec |
                    payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 | addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static ISpecification<ExternalCheque> ExternalChequeFullText(string text)
        {
            Specification<ExternalCheque> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var tellerSpec = new DirectSpecification<ExternalCheque>(c => c.Teller.Description.Contains(text));
                var numberSpec = new DirectSpecification<ExternalCheque>(c => c.Number.Contains(text));
                var drawerSpec = new DirectSpecification<ExternalCheque>(c => c.Drawer.Contains(text));
                var drawerBankSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBank.Contains(text));
                var drawerBankBranchSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBankBranch.Contains(text));
                var chequeTypeSpec = new DirectSpecification<ExternalCheque>(c => c.ChequeType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<ExternalCheque>(c => c.CreatedBy.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (tellerSpec | numberSpec | drawerSpec | drawerBankSpec | drawerBankBranchSpec | chequeTypeSpec | createdBySpec |
                    payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 | addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }

        public static ISpecification<ExternalCheque> ExternalChequeFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<ExternalCheque> specification = new DirectSpecification<ExternalCheque>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var tellerSpec = new DirectSpecification<ExternalCheque>(c => c.Teller.Description.Contains(text));
                var numberSpec = new DirectSpecification<ExternalCheque>(c => c.Number.Contains(text));
                var drawerSpec = new DirectSpecification<ExternalCheque>(c => c.Drawer.Contains(text));
                var drawerBankSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBank.Contains(text));
                var drawerBankBranchSpec = new DirectSpecification<ExternalCheque>(c => c.DrawerBankBranch.Contains(text));
                var chequeTypeSpec = new DirectSpecification<ExternalCheque>(c => c.ChequeType.Description.Contains(text));
                var createdBySpec = new DirectSpecification<ExternalCheque>(c => c.CreatedBy.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));
                var referenceSpec1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var referenceSpec2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var referenceSpec3 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var addressLandLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.LandLine.Contains(text));
                var addressMobileLineSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.MobileLine.Contains(text));
                var firstNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var addressEmail = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Email.Contains(text));
                var addressCity = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.City.Contains(text));
                var addressPostalCode = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.PostalCode.Contains(text));
                var addressAddressLine1 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine1.Contains(text));
                var addressAddressLine2 = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.AddressLine2.Contains(text));
                var addressStreet = new DirectSpecification<ExternalCheque>(c => c.CustomerAccount.Customer.Address.Street.Contains(text));

                specification &= (tellerSpec | numberSpec | drawerSpec | drawerBankSpec | drawerBankBranchSpec | chequeTypeSpec | createdBySpec |
                    payrollNumbersSpec | identificationNumberSpec | referenceSpec1 | referenceSpec2 | referenceSpec3 | addressLandLineSpec | addressMobileLineSpec | firstNameSpec | lastNameSpec | addressEmail | addressCity | addressPostalCode | addressAddressLine1 | addressAddressLine2 | addressStreet);
            }

            return specification;
        }
    }
}
