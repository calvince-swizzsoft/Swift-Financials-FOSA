using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditBatchDiscrepancyAgg
{
    public static class CreditBatchDiscrepancyFactory
    {
        public static CreditBatchDiscrepancy CreateCreditBatchDiscrepancy(Guid creditBatchId, string column1, string column2, string column3, string column4, string column5, string column6, string column7, string column8, string remarks)
        {
            var creditBatchDiscrepancy = new CreditBatchDiscrepancy();

            creditBatchDiscrepancy.GenerateNewIdentity();

            creditBatchDiscrepancy.CreditBatchId = creditBatchId;

            creditBatchDiscrepancy.Column1 = column1;

            creditBatchDiscrepancy.Column2 = column2;

            creditBatchDiscrepancy.Column3 = column3;

            creditBatchDiscrepancy.Column4 = column4;

            creditBatchDiscrepancy.Column5 = column5;

            creditBatchDiscrepancy.Column6 = column6;

            creditBatchDiscrepancy.Column7 = column7;

            creditBatchDiscrepancy.Column8 = column8;

            creditBatchDiscrepancy.Remarks = remarks;

            creditBatchDiscrepancy.CreatedDate = DateTime.Now;

            return creditBatchDiscrepancy;
        }
    }
}
