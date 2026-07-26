using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.LoanProductAppraisalProductAgg
{
    public static class LoanProductAppraisalProductFactory
    {
        public static LoanProductAppraisalProduct CreateLoanAppraisalProduct(Guid loanProductId, int productCode, Guid targetProductId, int purpose)
        {
            var loanAppraisalProduct = new LoanProductAppraisalProduct();

            loanAppraisalProduct.GenerateNewIdentity();

            loanAppraisalProduct.LoanProductId = loanProductId;

            loanAppraisalProduct.ProductCode = productCode;

            loanAppraisalProduct.TargetProductId = targetProductId;

            loanAppraisalProduct.Purpose = purpose;

            loanAppraisalProduct.CreatedDate = DateTime.Now;

            return loanAppraisalProduct;
        }
    }
}
