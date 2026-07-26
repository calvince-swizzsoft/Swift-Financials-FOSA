using Domain.MainBoundedContext.AccountsModule.Aggregates.ChartOfAccountAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.CustomerAccountAgg;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.MobileToBankRequestAgg
{
    public class MobileToBankRequest : Entity
    {
        public Guid? CustomerAccountId { get; set; }

        public virtual CustomerAccount CustomerAccount { get; private set; }

        public Guid? ChartOfAccountId { get; set; }

        public virtual ChartOfAccount ChartOfAccount { get; private set; }

        public string SystemTraceAuditNumber { get; set; }

        public string MSISDN { get; set; }

        public string BusinessShortCode { get; set; }

        public string InvoiceNumber { get; set; }

        public string TransID { get; set; }

        public decimal TransAmount { get; set; }

        public string ThirdPartyTransID { get; set; }

        public string TransTime { get; set; }

        public string BillRefNumber { get; set; }

        public decimal OrgAccountBalance { get; set; }

        public string KYCInfo { get; set; }

        public byte Status { get; set; }

        public string Remarks { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public byte RecordStatus { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public byte ReconciliationType { get; set; }
    }
}