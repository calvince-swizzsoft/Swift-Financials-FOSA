using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class WireTransferBatchEntryBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid WireTransferBatchId { get; set; }

        public Guid CustomerAccountId { get; set; }

        public decimal Amount { get; set; }

        public string Payee { get; set; }

        public string AccountNumber { get; set; }

        public string Reference { get; set; }

        public string ThirdPartyResponse { get; set; }

        public byte Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
