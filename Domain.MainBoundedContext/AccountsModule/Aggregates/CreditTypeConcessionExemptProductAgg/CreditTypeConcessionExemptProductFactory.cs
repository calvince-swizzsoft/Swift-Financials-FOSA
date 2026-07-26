using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.AccountsModule.Aggregates.CreditTypeConcessionExemptProductAgg
{
    public static class CreditTypeConcessionExemptProductFactory
    {
        public static CreditTypeConcessionExemptProduct CreateCreditTypeConcessionExemptProduct(Guid creditTypeId, int productCode, Guid targetProductId)
        {
            var creditTypeConcessionExemptProduct = new CreditTypeConcessionExemptProduct();

            creditTypeConcessionExemptProduct.GenerateNewIdentity();

            creditTypeConcessionExemptProduct.CreditTypeId = creditTypeId;

            creditTypeConcessionExemptProduct.ProductCode = (byte)productCode;

            creditTypeConcessionExemptProduct.TargetProductId = targetProductId;

            creditTypeConcessionExemptProduct.CreatedDate = DateTime.Now;

            return creditTypeConcessionExemptProduct;
        }
    }
}
