using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.JournalVoucherAgg
{
    public static class JournalVoucherSpecifications
    {
        public static Specification<JournalVoucher> DefaultSpec()
        {
            Specification<JournalVoucher> specification = new TrueSpecification<JournalVoucher>();

            return specification;
        }

        public static Specification<JournalVoucher> JournalVouchersWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<JournalVoucher> specification = new DirectSpecification<JournalVoucher>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var chartOfAccountSpec = new DirectSpecification<JournalVoucher>(c => c.ChartOfAccount.AccountName.Contains(text));

                var firstNameSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));

                var customerReference1Spec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var customerReference2Spec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var customerReference3Spec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var referenceSpec = new DirectSpecification<JournalVoucher>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<JournalVoucher>(c => c.CreatedBy.Contains(text));

                specification &= (chartOfAccountSpec | createdBySpec | referenceSpec |
                    firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | customerReference1Spec | customerReference2Spec | customerReference3Spec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var voucherNumberSpec = new DirectSpecification<JournalVoucher>(x => x.VoucherNumber == number);

                    specification |= voucherNumberSpec;
                }
            }

            return specification;
        }

        public static ISpecification<JournalVoucher> JournalVoucherWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<JournalVoucher> specification = new DirectSpecification<JournalVoucher>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var chartOfAccountSpec = new DirectSpecification<JournalVoucher>(c => c.ChartOfAccount.AccountName.Contains(text));

                var firstNameSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.FirstName.Contains(text));
                var lastNameSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.LastName.Contains(text));
                var payrollNumbersSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.PayrollNumbers.Contains(text));
                var identificationNumberSpec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Individual.IdentityCardNumber.Contains(text));

                var customerReference1Spec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Reference1.Contains(text));
                var customerReference2Spec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Reference2.Contains(text));
                var customerReference3Spec = new DirectSpecification<JournalVoucher>(c => c.CustomerAccount.Customer.Reference3.Contains(text));

                var referenceSpec = new DirectSpecification<JournalVoucher>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<JournalVoucher>(c => c.CreatedBy.Contains(text));

                specification &= (chartOfAccountSpec | createdBySpec | referenceSpec |
                    firstNameSpec | lastNameSpec | payrollNumbersSpec | identificationNumberSpec | customerReference1Spec | customerReference2Spec | customerReference3Spec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var voucherNumberSpec = new DirectSpecification<JournalVoucher>(x => x.VoucherNumber == number);

                    specification |= voucherNumberSpec;
                }
            }
            return specification;
        }

            public static Specification<JournalVoucher> JournalVoucherFullText(string text)
            {
                Specification<JournalVoucher> specification = DefaultSpec();

                if (!String.IsNullOrWhiteSpace(text))
                {
                    var descriptionSpec = new DirectSpecification<JournalVoucher>(c => c.PrimaryDescription.Contains(text));

                    specification &= (descriptionSpec);
                }

                return specification;
            }

    }
}
