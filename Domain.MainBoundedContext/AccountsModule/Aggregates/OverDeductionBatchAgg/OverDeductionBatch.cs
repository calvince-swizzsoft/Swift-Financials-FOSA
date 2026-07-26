using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchEntryAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchAgg
{
    public class OverDeductionBatch : Domain.Seedwork.Entity
    {
        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public int BatchNumber { get; set; }

        public string Reference { get; set; }

        public decimal TotalValue { get; set; }

        public byte Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        

        

        HashSet<OverDeductionBatchEntry> _overDeductionBatchEntries;
        public virtual ICollection<OverDeductionBatchEntry> OverDeductionBatchEntries
        {
            get
            {
                if (_overDeductionBatchEntries == null)
                {
                    _overDeductionBatchEntries = new HashSet<OverDeductionBatchEntry>();
                }
                return _overDeductionBatchEntries;
            }
            private set
            {
                _overDeductionBatchEntries = new HashSet<OverDeductionBatchEntry>(value);
            }
        }
    }
}
