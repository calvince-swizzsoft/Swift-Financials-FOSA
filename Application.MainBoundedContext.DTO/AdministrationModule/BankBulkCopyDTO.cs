using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class BankBulkCopyDTO
    {
        public Guid Id { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
