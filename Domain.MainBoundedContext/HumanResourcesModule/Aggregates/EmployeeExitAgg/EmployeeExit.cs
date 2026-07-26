using Domain.MainBoundedContext.AdministrationModule.Aggregates.BranchAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeExitAgg
{
    public class EmployeeExit : Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid BranchId { get; set; }

        public virtual Branch Branch { get; private set; }

        public string Reason { get; set; }

        public byte Type { get; set; }

        public byte Status { get; set; }

        public string AuditedBy { get; set; }

        public string AuditRemarks { get; set; }

        public DateTime? AuditedDate { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }

        public virtual File File { get; set; }
    }
}
