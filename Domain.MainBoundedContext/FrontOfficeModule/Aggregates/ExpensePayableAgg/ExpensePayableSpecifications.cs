using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Extensions;
using Infrastructure.Crosscutting.Framework.Utils;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg
{
    public static class ExpensePayableSpecifications
    {
        public static Specification<ExpensePayable> DefaultSpec()
        {
            Specification<ExpensePayable> specification = new TrueSpecification<ExpensePayable>();

            return specification;
        }

        public static Specification<ExpensePayable> ExpensePayablesWithStatus(int status, DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<ExpensePayable> specification = new DirectSpecification<ExpensePayable>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var createdBySpec = new DirectSpecification<ExpensePayable>(c => c.CreatedBy.Contains(text));
                var referenceSpec = new DirectSpecification<ExpensePayable>(c => c.Remarks.Contains(text));

                specification &= (createdBySpec | referenceSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var voucherNumberSpec = new DirectSpecification<ExpensePayable>(x => x.VoucherNumber == number);

                    specification |= voucherNumberSpec;
                }
            }

            return specification;
        }

        public static Specification<ExpensePayable> ExpensePayablesWithText( string text)
        {

            Specification<ExpensePayable> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var createdBySpec = new DirectSpecification<ExpensePayable>(c => c.CreatedBy.Contains(text));
                var referenceSpec = new DirectSpecification<ExpensePayable>(c => c.Remarks.Contains(text));

                specification &= (createdBySpec | referenceSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var voucherNumberSpec = new DirectSpecification<ExpensePayable>(x => x.VoucherNumber == number);

                    specification |= voucherNumberSpec;
                }
            }

            return specification;
        }

        public static ISpecification<ExpensePayable> ExpensePayableWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<ExpensePayable> specification = new DirectSpecification<ExpensePayable>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var createdBySpec = new DirectSpecification<ExpensePayable>(c => c.CreatedBy.Contains(text));
                var referenceSpec = new DirectSpecification<ExpensePayable>(c => c.Remarks.Contains(text));

                specification &= (createdBySpec | referenceSpec);

                int number = default(int);
                if (int.TryParse(text.StripPunctuation(), out number))
                {
                    var voucherNumberSpec = new DirectSpecification<ExpensePayable>(x => x.VoucherNumber == number);

                    specification |= voucherNumberSpec;
                }
            }

            return specification;
        }
    }
}
