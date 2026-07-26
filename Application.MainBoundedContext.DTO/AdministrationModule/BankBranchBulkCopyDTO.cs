using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class BankBranchBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid BankId { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public string Address_AddressLine1 { get; set; }

        public string Address_AddressLine2 { get; set; }

        public string Address_Street { get; set; }

        public string Address_PostalCode { get; set; }

        public string Address_City { get; set; }

        public string Address_Email { get; set; }

        public string Address_LandLine { get; set; }

        public string Address_MobileLine { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
