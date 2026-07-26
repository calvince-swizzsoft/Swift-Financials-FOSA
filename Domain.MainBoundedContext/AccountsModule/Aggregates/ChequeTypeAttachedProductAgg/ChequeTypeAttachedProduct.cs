using Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAgg;
using Infrastructure.Crosscutting.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAttachedProductAgg
{
    public class ChequeTypeAttachedProduct : Domain.Seedwork.Entity
    {
        public Guid ChequeTypeId { get; set; }

        public virtual ChequeType ChequeType { get; private set; }

        [Index("IX_ChequeTypeAttachedProduct_ProductCode")]
        public byte ProductCode { get; set; }

        [Index("IX_ChequeTypeAttachedProduct_TargetProductId")]
        public Guid TargetProductId { get; set; }

        
    }
}
