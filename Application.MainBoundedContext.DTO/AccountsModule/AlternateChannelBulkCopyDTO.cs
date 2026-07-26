using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AlternateChannelBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid CustomerAccountId { get; set; }

        public int Type { get; set; }

        public string CardNumber { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime Expires { get; set; }

        public decimal DailyLimit { get; set; }

        public string Remarks { get; set; }

        public bool IsLocked { get; set; }

        public int RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
