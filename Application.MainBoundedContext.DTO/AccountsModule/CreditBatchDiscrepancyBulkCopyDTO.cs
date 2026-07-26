using System;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CreditBatchDiscrepancyBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CreditBatchId { get; set; }

        public string Column1 { get; set; }

        public string Column2 { get; set; }

        public string Column3 { get; set; }

        public string Column4 { get; set; }

        public string Column5 { get; set; }

        public string Column6 { get; set; }

        public string Column7 { get; set; }

        public string Column8 { get; set; }

        public string Remarks { get; set; }

        public byte Status { get; set; }

        public string PostedBy { get; set; }

        public DateTime? PostedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
