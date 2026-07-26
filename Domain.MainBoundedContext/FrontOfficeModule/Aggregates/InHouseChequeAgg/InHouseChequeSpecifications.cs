using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.InHouseChequeAgg
{
    public static class InHouseChequeSpecifications
    {
        public static Specification<InHouseCheque> DefaultSpec()
        {
            Specification<InHouseCheque> specification = new TrueSpecification<InHouseCheque>();

            return specification;
        }

        public static Specification<InHouseCheque> UnPrintedInHouseChequesWithBranchId(Guid branchId, string text)
        {
            Specification<InHouseCheque> specification = new DirectSpecification<InHouseCheque>(x => x.BranchId == branchId && !x.IsPrinted && x.PrintedDate == null);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var payeeSpec = new DirectSpecification<InHouseCheque>(c => c.Payee.Contains(text));
                var printedNumberSpec = new DirectSpecification<InHouseCheque>(c => c.PrintedNumber.Contains(text));
                var referenceSpec = new DirectSpecification<InHouseCheque>(c => c.Reference.Contains(text));

                specification &= (payeeSpec | printedNumberSpec | referenceSpec);
            }

            return specification;
        }

        public static Specification<InHouseCheque> InHouseChequeFullText(string text)
        {
            Specification<InHouseCheque> specification = DefaultSpec();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var payeeSpec = new DirectSpecification<InHouseCheque>(c => c.Payee.Contains(text));
                var printedNumberSpec = new DirectSpecification<InHouseCheque>(c => c.PrintedNumber.Contains(text));
                var referenceSpec = new DirectSpecification<InHouseCheque>(c => c.Reference.Contains(text));

                specification &= (payeeSpec | printedNumberSpec | referenceSpec);
            }

            return specification;
        }

        public static Specification<InHouseCheque> InHouseChequeFullText(DateTime startDate, DateTime endDate, string text)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<InHouseCheque> specification = new DirectSpecification<InHouseCheque>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var payeeSpec = new DirectSpecification<InHouseCheque>(c => c.Payee.Contains(text));
                var printedNumberSpec = new DirectSpecification<InHouseCheque>(c => c.PrintedNumber.Contains(text));
                var referenceSpec = new DirectSpecification<InHouseCheque>(c => c.Reference.Contains(text));

                specification &= (payeeSpec | printedNumberSpec | referenceSpec);
            }

            return specification;
        }

        public static Specification<InHouseCheque> ReconcileableInHouseCheque(DateTime startDate, DateTime endDate, string chequeNumber, decimal amount)
        {
            endDate = UberUtil.AdjustTimeSpan(endDate);

            Specification<InHouseCheque> specification = new DirectSpecification<InHouseCheque>(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Amount == amount);

            if (!String.IsNullOrWhiteSpace(chequeNumber))
            {
                var printedNumberSpec = new DirectSpecification<InHouseCheque>(c => c.PrintedNumber == chequeNumber);

                specification &= (printedNumberSpec);
            }

            return specification;
        }
    }
}
