using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CashFlowMappingAgg
{
    public class CashFlowMapping : Entity
    {
        protected CashFlowMapping() { }

        internal CashFlowMapping(Guid chartOfAccountId, string section, string line, string user)
        {
            if (chartOfAccountId == Guid.Empty)
                throw new ArgumentException("Select a G/L account.");
            ChartOfAccountId = chartOfAccountId;
            UpdateClassification(section, line, user);
        }

        public Guid ChartOfAccountId { get; private set; }
        public virtual ChartOfAccount ChartOfAccount { get; private set; }
        public string Section { get; private set; }
        public string Line { get; private set; }
        public string ModifiedBy { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        public void UpdateClassification(string section, string line, string user)
        {
            if (section != "Cash" && section != "Operating" && section != "Investing" && section != "Financing" && section != "Exchange")
                throw new ArgumentException("Select a valid cash-flow section.");
            if (string.IsNullOrWhiteSpace(line) || line.Trim().Length > 120)
                throw new ArgumentException("Enter a report line of up to 120 characters.");
            Section = section;
            Line = line.Trim();
            ModifiedBy = user;
            ModifiedDate = DateTime.Now;
        }
    }
}
