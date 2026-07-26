using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DepartmentAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FileRegisterAgg;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FileMovementHistoryAgg
{
    public class FileMovementHistory : Domain.Seedwork.Entity
    {
        public Guid FileRegisterId { get; set; }

        public virtual FileRegister FileRegister { get; private set; }

        public Guid SourceDepartmentId { get; set; }

        public virtual Department SourceDepartment { get; private set; }

        public Guid DestinationDepartmentId { get; set; }

        public virtual Department DestinationDepartment { get; private set; }

        public string Remarks { get; set; }

        public string Carrier { get; set; }

        public string Sender { get; set; }

        public DateTime? SendDate { get; set; }

        public string Recipient { get; set; }

        public DateTime? ReceiveDate { get; set; }
    }
}
