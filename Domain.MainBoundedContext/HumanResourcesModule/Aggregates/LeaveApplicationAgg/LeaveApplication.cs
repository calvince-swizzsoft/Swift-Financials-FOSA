using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveTypeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.LeaveApplicationAgg
{
    public class LeaveApplication : Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public Guid? LeaveTypeId { get; set; }

        public virtual LeaveType LeaveType { get; private set; }

        public virtual Duration Duration { get; set; }

        public string Reason { get; set; }

        public byte Status { get; set; }
        
        public decimal Balance { get; set; }

        public string AuthorizedBy { get; set; }

        public string AuthorizationRemarks { get; set; }

        public DateTime? AuthorizedDate { get; set; }

        public string RecalledBy { get; set; }

        public string RecallRemarks { get; set; }

        public DateTime? RecalledDate { get; set; }

        public string DocumentNumber { get; set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }

        public virtual File File { get; set; }
    }
}
