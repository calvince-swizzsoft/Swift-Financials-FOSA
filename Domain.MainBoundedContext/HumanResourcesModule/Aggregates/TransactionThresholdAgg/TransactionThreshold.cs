using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.DesignationAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.HumanResourcesModule.Aggregates.TransactionThresholdAgg
{
    public class TransactionThreshold : Entity
    {
        public Guid DesignationId { get; set; }

        public virtual Designation Designation { get; private set; }

        [Index("IX_TransactionThreshold_Type")]
        public short Type { get; set; }

        public decimal Threshold { get; set; }

        

        
    }
}
