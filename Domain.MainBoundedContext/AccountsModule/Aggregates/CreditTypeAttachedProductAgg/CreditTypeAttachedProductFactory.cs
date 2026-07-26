using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeAttachedProductAgg
{
    public static class CreditTypeAttachedProductFactory
    {
        public static CreditTypeAttachedProduct CreateCreditTypeAttachedProduct(Guid creditTypeId, int productCode, Guid targetProductId)
        {
            var creditTypeAttachedProduct = new CreditTypeAttachedProduct();

            creditTypeAttachedProduct.GenerateNewIdentity();

            creditTypeAttachedProduct.CreditTypeId = creditTypeId;

            creditTypeAttachedProduct.ProductCode = (byte)productCode;

            creditTypeAttachedProduct.TargetProductId = targetProductId;

            creditTypeAttachedProduct.CreatedDate = DateTime.Now;

            return creditTypeAttachedProduct;
        }
    }
}
