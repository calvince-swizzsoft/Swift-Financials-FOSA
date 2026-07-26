using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDisciplinaryCaseAgg
{
    public class EmployeeDisciplinaryCase:Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public DateTime IncidentDate { get; set; }

        public byte Type { get; set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }

        public virtual File File { get; set; }

        public string Remarks { get; set; }
    }
}
