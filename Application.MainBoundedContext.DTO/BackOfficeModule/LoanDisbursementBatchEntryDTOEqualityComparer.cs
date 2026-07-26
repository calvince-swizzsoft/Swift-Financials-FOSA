using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class LoanDisbursementBatchEntryDTOEqualityComparer : EqualityComparer<LoanDisbursementBatchEntryDTO>
    {
        public override bool Equals(LoanDisbursementBatchEntryDTO x, LoanDisbursementBatchEntryDTO y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            if (x.LoanCaseId == y.LoanCaseId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode(LoanDisbursementBatchEntryDTO obj)
        {
            return obj.LoanCaseId.GetHashCode();
        }
    }
}
