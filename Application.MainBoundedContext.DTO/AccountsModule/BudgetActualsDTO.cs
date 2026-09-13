using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class BudgetActualsDTO
    {
        public BudgetDTO Budget { get; set; }
        public DateTime AsAt { get; set; }
        public List<BudgetActualLineDTO> Lines { get; set; }
        public List<BudgetActualTotalDTO> Totals { get; set; }
    }
    public class BudgetActualLineDTO
    {
        public Guid TargetId { get; set; }
        public string Section { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }
        public decimal Actual { get; set; }
        public decimal Difference { get { return Budget - Actual; } }
        public decimal? Percentage { get { return Budget == 0 ? (decimal?)null : Actual / Budget; } }
        public bool Unbudgeted { get { return Budget == 0; } }
    }
    public class BudgetActualTotalDTO
    {
        public string Section { get; set; }
        public decimal Budget { get; set; }
        public decimal Actual { get; set; }
        public decimal Difference { get { return Budget - Actual; } }
        public decimal? Percentage { get { return Budget == 0 ? (decimal?)null : Actual / Budget; } }
    }
}
