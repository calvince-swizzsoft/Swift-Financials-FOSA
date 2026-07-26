using Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAgg;
using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeConcessionExemptProductAgg
{
    public class CreditTypeConcessionExemptProduct : Entity
    {
        public Guid CreditTypeId { get; set; }

        public virtual CreditType CreditType { get; private set; }

        [Index("IX_CreditTypeConcessionExemptProduct_ProductCode")]
        public byte ProductCode { get; set; }

        [Index("IX_CreditTypeConcessionExemptProduct_TargetProductId")]
        public Guid TargetProductId { get; set; }

        
    }
}
