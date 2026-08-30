using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeAgg;
using Domain.Seedwork;
using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg
{
    public class CashTransferRequest : Entity
    {
        public Guid? EmployeeId { get; set; }

        public virtual Employee Employee { get; private set; }

        public byte Status { get; set; }

        public bool Utilized { get; set; }

        public decimal Amount { get; set; }

        public virtual Denomination Denomination { get; set; }

        public string Reference { get; set; }

        public string Remarks { get; set; }

        public string AcknowledgedBy { get; set; }

        public DateTime? AcknowledgedDate { get; set; }
    }
}
