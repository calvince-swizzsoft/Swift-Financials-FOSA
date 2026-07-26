using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.DebitBatchEntryAgg
{
    public static class DebitBatchEntrySpecifications
    {
        public static Specification<DebitBatchEntry> DefaultSpec()
        {
            Specification<DebitBatchEntry> specification = new TrueSpecification<DebitBatchEntry>();

            return specification;
        }

        public static Specification<DebitBatchEntry> DebitBatchEntryWithDebitBatchId(Guid debitBatchId, string text)
        {
            Specification<DebitBatchEntry> specification = DefaultSpec();

            if (debitBatchId != null && debitBatchId != Guid.Empty)
            {
                var debitBatchIdSpec = new DirectSpecification<DebitBatchEntry>(c => c.DebitBatchId == debitBatchId);

                specification &= debitBatchIdSpec;
            }

            return specification;
        }

        public static Specification<DebitBatchEntry> DebitBatchEntryWithCustomerId(Guid customerId)
        {
            Specification<DebitBatchEntry> specification = DefaultSpec();

            if (customerId != null && customerId != Guid.Empty)
            {
                var customerIdSpec = new DirectSpecification<DebitBatchEntry>(c => c.CustomerAccount.CustomerId == customerId);

                specification &= customerIdSpec;
            }

            return specification;
        }

        public static Specification<DebitBatchEntry> PostedDebitBatchEntryWithDebitBatchId(Guid debitBatchId)
        {
            Specification<DebitBatchEntry> specification = new DirectSpecification<DebitBatchEntry>(x => x.DebitBatchId == debitBatchId && x.Status == (int)BatchEntryStatus.Posted);

            return specification;
        }

        public static Specification<DebitBatchEntry> QueableDebitBatchEntries()
        {
            Specification<DebitBatchEntry> specification = new DirectSpecification<DebitBatchEntry>(c => c.Status == (int)BatchEntryStatus.Pending && c.DebitBatch.Status == (int)BatchStatus.Posted);

            return specification;
        }
    }
}
