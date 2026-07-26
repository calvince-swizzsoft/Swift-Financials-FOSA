using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class JournalReversalBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid JournalReversalBatchId { get; set; }

        public Guid? JournalId { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
