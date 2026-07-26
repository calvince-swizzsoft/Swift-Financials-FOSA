using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCollateralAgg
{
    public static class LoanCollateralFactory
    {
        public static LoanCollateral CreateLoanCollateral(Guid customerDocumentId, Guid loanCaseId, decimal value)
        {
            var loanCollateral = new LoanCollateral();

            loanCollateral.GenerateNewIdentity();

            loanCollateral.CustomerDocumentId = customerDocumentId;

            loanCollateral.LoanCaseId = loanCaseId;

            loanCollateral.Value = value;

            loanCollateral.CreatedDate = DateTime.Now;

            return loanCollateral;
        }
    }
}
