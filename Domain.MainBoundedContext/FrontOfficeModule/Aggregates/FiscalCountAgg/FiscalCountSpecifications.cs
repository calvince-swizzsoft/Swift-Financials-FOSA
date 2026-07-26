using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg
{
    public static class FiscalCountSpecifications
    {
        public static Specification<FiscalCount> DefaultSpec()
        {
            Specification<FiscalCount> specification = new TrueSpecification<FiscalCount>();

            return specification;
        }

        public static ISpecification<FiscalCount> FiscalCountFullText(string text)
        {
            Specification<FiscalCount> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var chartOfAccountSpec = new DirectSpecification<FiscalCount>(c => c.ChartOfAccount.AccountName.Contains(text));
                var primaryDescriptionSpec = new DirectSpecification<FiscalCount>(c => c.PrimaryDescription.Contains(text));
                var secondaryDescriptionSpec = new DirectSpecification<FiscalCount>(c => c.SecondaryDescription.Contains(text));
                var referenceSpec = new DirectSpecification<FiscalCount>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<FiscalCount>(c => c.CreatedBy.Contains(text));

                specification &= (chartOfAccountSpec | primaryDescriptionSpec | secondaryDescriptionSpec | createdBySpec | referenceSpec);
            }

            return specification;
        }

        public static ISpecification<FiscalCount> FiscalCountWithDateRangeAndFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<FiscalCount> specification = new DirectSpecification<FiscalCount>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var chartOfAccountSpec = new DirectSpecification<FiscalCount>(c => c.ChartOfAccount.AccountName.Contains(text));
                var primaryDescriptionSpec = new DirectSpecification<FiscalCount>(c => c.PrimaryDescription.Contains(text));
                var secondaryDescriptionSpec = new DirectSpecification<FiscalCount>(c => c.SecondaryDescription.Contains(text));
                var referenceSpec = new DirectSpecification<FiscalCount>(c => c.Reference.Contains(text));
                var createdBySpec = new DirectSpecification<FiscalCount>(c => c.CreatedBy.Contains(text));

                specification &= (chartOfAccountSpec | primaryDescriptionSpec | secondaryDescriptionSpec | createdBySpec | referenceSpec);
            }

            return specification;
        }

        public static ISpecification<FiscalCount> FiscalCountWithDateRangeAndTransactionCodeAndApplicationUserName(DateTime startDate, DateTime endDate, int transactionCode, string applicationUserName)
        {
            Specification<FiscalCount> specification = DefaultSpec();

            endDate = UberUtil.AdjustTimeSpan(endDate);

            specification &= new DirectSpecification<FiscalCount>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.TransactionCode == transactionCode && x.CreatedBy == applicationUserName);

            return specification;
        }
    }
}
