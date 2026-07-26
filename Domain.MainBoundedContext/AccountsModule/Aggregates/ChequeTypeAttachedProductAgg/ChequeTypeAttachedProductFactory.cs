using System;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.ChequeTypeAttachedProductAgg
{
    public static class ChequeTypeAttachedProductFactory
    {
        public static ChequeTypeAttachedProduct CreateChequeTypeAttachedProduct(Guid chequeTypeId, int productCode, Guid targetProductId)
        {
            var creditTypeAttachedProduct = new ChequeTypeAttachedProduct();

            creditTypeAttachedProduct.GenerateNewIdentity();

            creditTypeAttachedProduct.ChequeTypeId = chequeTypeId;

            creditTypeAttachedProduct.ProductCode = (byte)productCode;

            creditTypeAttachedProduct.TargetProductId = targetProductId;

            creditTypeAttachedProduct.CreatedDate = DateTime.Now;

            return creditTypeAttachedProduct;
        }
    }
}
