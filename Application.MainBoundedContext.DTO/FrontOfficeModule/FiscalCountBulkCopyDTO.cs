using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class FiscalCountBulkCopyDTO
    {
        public Guid Id { get; set; }

        public Guid PostingPeriodId { get; set; }

        public Guid BranchId { get; set; }

        public Guid ChartOfAccountId { get; set; }

        public string PrimaryDescription { get; set; }

        public string SecondaryDescription { get; set; }

        public string Reference { get; set; }

        public decimal Denomination_OneThousandValue { get; set; }

        public decimal Denomination_FiveHundredValue { get; set; }

        public decimal Denomination_TwoHundredValue { get; set; }

        public decimal Denomination_OneHundredValue { get; set; }

        public decimal Denomination_FiftyValue { get; set; }

        public decimal Denomination_FourtyValue { get; set; }

        public decimal Denomination_TwentyValue { get; set; }

        public decimal Denomination_TenValue { get; set; }

        public decimal Denomination_FiveValue { get; set; }

        public decimal Denomination_OneValue { get; set; }

        public decimal Denomination_FiftyCentValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
