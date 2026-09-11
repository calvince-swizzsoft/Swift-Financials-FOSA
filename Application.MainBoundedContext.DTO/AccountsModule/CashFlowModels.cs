using System;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CashFlowMappingDTO
    {
        public Guid ChartOfAccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Section { get; set; }
        public string Line { get; set; }
    }

    public class CashFlowRowDTO
    {
        public string Section { get; set; }
        public string Line { get; set; }
        public decimal Receipts { get; set; }
        public decimal Payments { get; set; }
        public decimal Net { get { return Receipts - Payments; } }
        public int DetailCount { get; set; }
    }

    public class CashFlowDetailDTO : CashFlowRowDTO
    {
        public Guid JournalId { get; set; }
        public DateTime ValueDate { get; set; }
        public string Reference { get; set; }
        public string Narration { get; set; }
        public int TotalCount { get; set; }
    }

    public class CashFlowReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? BranchId { get; set; }
        public decimal OpeningCash { get; set; }
        public decimal ClosingCash { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal ExchangeEffects { get; set; }
        public decimal UnclassifiedNet { get; set; }
        public decimal ReconciliationDifference { get; set; }
        public bool IsComplete { get; set; }
        public List<CashFlowRowDTO> Rows { get; set; }
    }
}
