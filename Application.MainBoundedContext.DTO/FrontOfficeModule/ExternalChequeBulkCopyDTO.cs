using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class ExternalChequeBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid? TellerId { get; set; }

        public Guid? CustomerAccountId { get; set; }

        public string Number { get; set; }

        public decimal Amount { get; set; }

        public string Drawer { get; set; }

        public string DrawerBank { get; set; }

        public string DrawerBankBranch { get; set; }

        public DateTime WriteDate { get; set; }

        public string ChequeType { get; set; }

        public DateTime MaturityDate { get; set; }

        public bool IsCleared { get; set; }

        public string ClearedBy { get; set; }

        public DateTime? ClearedDate { get; set; }

        public bool IsBanked { get; set; }

        public string BankedBy { get; set; }

        public DateTime? BankedDate { get; set; }

        public bool IsTransferred { get; set; }

        public string TransferredBy { get; set; }

        public DateTime? TransferredDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
