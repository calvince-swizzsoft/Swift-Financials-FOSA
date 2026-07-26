using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg
{
    public class CustomerDocument : Entity
    {
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; private set; }
        
        public int Type { get; set; }

        public virtual Collateral Collateral { get; set; }

        public string FileName { get; set; }

        public string FileTitle { get; set; }

        public string FileDescription { get; set; }

        public string FileMIMEType { get; set; }

        public virtual File File { get; set; }

        public byte RecordStatus { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
