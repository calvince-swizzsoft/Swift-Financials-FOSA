using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDocumentAgg
{
    public class EmployeeDocument : Entity
    {
        public Guid EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }
        public byte[] FileBuffer { get; set; }


        public virtual File File { get; set; }
    }
}
