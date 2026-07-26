using Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAgg;
using Domain.Seedwork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAttachedProductAgg
{
    public class FixedDepositTypeAttachedProduct : Entity
    {
        public Guid FixedDepositTypeId { get; set; }

        public virtual FixedDepositType FixedDepositType { get; private set; }

        [Index("IX_FixedDepositTypeAttachedProduct_ProductCode")]
        public byte ProductCode { get; set; }

        [Index("IX_FixedDepositTypeAttachedProduct_TargetProductId")]
        public Guid TargetProductId { get; set; }
    }
}
