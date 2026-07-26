using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.OverDeductionBatchDiscrepancyAgg
{
    public static class OverDeductionBatchDiscrepancyFactory
    {
        public static OverDeductionBatchDiscrepancy CreateOverDeductionBatchDiscrepancy(Guid overDeductionBatchId, string column1, string column2, string column3, string column4, string column5, string column6, string column7, string column8, string column9, string column10, string column11, string column12, string remarks)
        {
            var overDeductionBatchDiscrepancy = new OverDeductionBatchDiscrepancy();

            overDeductionBatchDiscrepancy.GenerateNewIdentity();

            overDeductionBatchDiscrepancy.OverDeductionBatchId = overDeductionBatchId;

            overDeductionBatchDiscrepancy.Column1 = column1;

            overDeductionBatchDiscrepancy.Column2 = column2;

            overDeductionBatchDiscrepancy.Column3 = column3;

            overDeductionBatchDiscrepancy.Column4 = column4;

            overDeductionBatchDiscrepancy.Column5 = column5;

            overDeductionBatchDiscrepancy.Column6 = column6;

            overDeductionBatchDiscrepancy.Column7 = column7;

            overDeductionBatchDiscrepancy.Column8 = column8;

            overDeductionBatchDiscrepancy.Column9 = column9;

            overDeductionBatchDiscrepancy.Column10 = column10;

            overDeductionBatchDiscrepancy.Column11 = column11;

            overDeductionBatchDiscrepancy.Column12 = column12;

            overDeductionBatchDiscrepancy.Remarks = remarks;

            overDeductionBatchDiscrepancy.CreatedDate = DateTime.Now;

            return overDeductionBatchDiscrepancy;
        }
    }
}
