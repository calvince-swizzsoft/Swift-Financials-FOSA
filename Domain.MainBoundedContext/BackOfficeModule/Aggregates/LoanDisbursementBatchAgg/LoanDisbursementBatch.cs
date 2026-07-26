using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.DataAttachmentPeriodAgg;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchEntryAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanDisbursementBatchAgg
{
    public class LoanDisbursementBatch : Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public Guid? DataAttachmentPeriodId { get; set; }

        public virtual DataAttachmentPeriod DataAttachmentPeriod { get; private set; }

        [Index("IX_LoanDisbursementBatch_BatchNumber")]
        public int BatchNumber { get; set; }

        public byte Type { get; set; }

        public byte LoanProductCategory { get; set; }

        public string Reference { get; set; }

        public byte Priority { get; set; }

        public byte Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }
        
        HashSet<LoanDisbursementBatchEntry> _loanDisbursementBatchEntries;
        public virtual ICollection<LoanDisbursementBatchEntry> LoanDisbursementBatchEntries
        {
            get
            {
                if (_loanDisbursementBatchEntries == null)
                {
                    _loanDisbursementBatchEntries = new HashSet<LoanDisbursementBatchEntry>();
                }
                return _loanDisbursementBatchEntries;
            }
            private set
            {
                _loanDisbursementBatchEntries = new HashSet<LoanDisbursementBatchEntry>(value);
            }
        }
    }
}
