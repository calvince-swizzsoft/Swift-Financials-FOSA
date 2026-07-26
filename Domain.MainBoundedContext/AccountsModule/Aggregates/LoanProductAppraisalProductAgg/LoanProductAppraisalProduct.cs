using Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAgg;
using Domain.Seedwork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAppraisalProductAgg
{
    public class LoanProductAppraisalProduct : Entity
    {
        public Guid LoanProductId { get; set; }

        public virtual LoanProduct LoanProduct { get; private set; }

        [Index("IX_LoanProductAppraisalProduct_ProductCode")]
        public int ProductCode { get; set; }

        [Index("IX_LoanProductAppraisalProduct_TargetProductId")]
        public Guid TargetProductId { get; set; }

        public int Purpose { get; set; }
    }
}
