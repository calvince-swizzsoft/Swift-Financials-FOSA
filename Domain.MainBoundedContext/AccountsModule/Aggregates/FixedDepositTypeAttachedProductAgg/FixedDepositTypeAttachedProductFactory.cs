using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.FixedDepositTypeAttachedProductAgg
{
    public static class FixedDepositTypeAttachedProductFactory
    {
        public static FixedDepositTypeAttachedProduct CreateFixedDepositTypeAttachedProduct(Guid chequeTypeId, int productCode, Guid targetProductId)
        {
            var fixedDepositTypeAttachedProduct = new FixedDepositTypeAttachedProduct();

            fixedDepositTypeAttachedProduct.GenerateNewIdentity();

            fixedDepositTypeAttachedProduct.FixedDepositTypeId = chequeTypeId;

            fixedDepositTypeAttachedProduct.ProductCode = (byte)productCode;

            fixedDepositTypeAttachedProduct.TargetProductId = targetProductId;

            fixedDepositTypeAttachedProduct.CreatedDate = DateTime.Now;

            return fixedDepositTypeAttachedProduct;
        }
    }
}
