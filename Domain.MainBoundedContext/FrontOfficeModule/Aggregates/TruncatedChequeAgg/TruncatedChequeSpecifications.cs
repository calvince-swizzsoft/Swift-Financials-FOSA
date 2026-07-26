using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg
{
    public static class TruncatedChequeSpecifications
    {
        public static Specification<TruncatedCheque> DefaultSpec()
        {
            Specification<TruncatedCheque> specification = new TrueSpecification<TruncatedCheque>();

            return specification;
        }

        public static Specification<TruncatedCheque> TruncatedChequeFullText(Guid electronicJournalId, string text)
        {
            Specification<TruncatedCheque> specification = new DirectSpecification<TruncatedCheque>(x => x.ElectronicJournalId == electronicJournalId);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var serialNumberSpec = new DirectSpecification<TruncatedCheque>(c => c.SerialNumber.Contains(text));
                var destinationAccountAccountSpec = new DirectSpecification<TruncatedCheque>(c => c.DestinationAccountAccount.Contains(text));
                var documentReferenceNumberSpec = new DirectSpecification<TruncatedCheque>(c => c.DocumentReferenceNumber.Contains(text));
                var createdBySpec = new DirectSpecification<TruncatedCheque>(c => c.CreatedBy.Contains(text));

                specification &= (serialNumberSpec | destinationAccountAccountSpec | documentReferenceNumberSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<TruncatedCheque> TruncatedChequeFullText(Guid electronicJournalId, int status, string text)
        {
            Specification<TruncatedCheque> specification = new DirectSpecification<TruncatedCheque>(x => x.ElectronicJournalId == electronicJournalId && x.Status == status);

            if (!String.IsNullOrWhiteSpace(text))
            {
                var serialNumberSpec = new DirectSpecification<TruncatedCheque>(c => c.SerialNumber.Contains(text));
                var destinationAccountAccountSpec = new DirectSpecification<TruncatedCheque>(c => c.DestinationAccountAccount.Contains(text));
                var documentReferenceNumberSpec = new DirectSpecification<TruncatedCheque>(c => c.DocumentReferenceNumber.Contains(text));
                var createdBySpec = new DirectSpecification<TruncatedCheque>(c => c.CreatedBy.Contains(text));

                specification &= (serialNumberSpec | destinationAccountAccountSpec | documentReferenceNumberSpec | createdBySpec);
            }

            return specification;
        }

        public static Specification<TruncatedCheque> ProcessedTruncatedChequeWithElectronicJournalId(Guid electronicJournalId)
        {
            Specification<TruncatedCheque> specification = new DirectSpecification<TruncatedCheque>(x => x.ElectronicJournalId == electronicJournalId && x.Status == (int)TruncatedChequeStatus.Processed);

            return specification;
        }

        public static Specification<TruncatedCheque> UnPaidTruncatedChequeWithElectronicJournalId(Guid electronicJournalId)
        {
            Specification<TruncatedCheque> specification = new DirectSpecification<TruncatedCheque>(x => x.ElectronicJournalId == electronicJournalId && x.Status == (int)TruncatedChequeStatus.Processed && x.UnPaidCode != 0);

            return specification;
        }
    }
}
