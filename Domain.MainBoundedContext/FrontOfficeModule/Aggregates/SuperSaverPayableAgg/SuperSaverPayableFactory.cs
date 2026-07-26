using System;

namespace Domain.MainBoundedContext.FrontOfficeModule.Aggregates.SuperSaverPayableAgg
{
    public static class SuperSaverPayableFactory
    {
        public static SuperSaverPayable CreateSuperSaverPayable(Guid customerAccountId, Guid branchId, decimal bookBalance, decimal amount, decimal withholdingTaxAmount, int status, string remarks)
        {
            var superSaverPayable = new SuperSaverPayable();

            superSaverPayable.GenerateNewIdentity();

            superSaverPayable.CustomerAccountId = customerAccountId;

            superSaverPayable.BranchId = branchId;

            superSaverPayable.BookBalance = bookBalance;

            superSaverPayable.Amount = amount;

            superSaverPayable.WithholdingTaxAmount = withholdingTaxAmount;

            superSaverPayable.Status = (byte)status;

            superSaverPayable.Remarks = remarks;

            superSaverPayable.CreatedDate = DateTime.Now;

            return superSaverPayable;
        }
    }
}
