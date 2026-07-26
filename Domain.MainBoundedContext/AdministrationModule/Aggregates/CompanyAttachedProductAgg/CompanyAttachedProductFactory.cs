using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAttachedProductAgg
{
    public static class CompanyAttachedProductFactory
    {
        public static CompanyAttachedProduct CreateCompanyAttachedProduct(Guid companyId, int productCode, Guid targetProductId)
        {
            var companyAttachedProduct = new CompanyAttachedProduct();

            companyAttachedProduct.GenerateNewIdentity();

            companyAttachedProduct.CompanyId = companyId;

            companyAttachedProduct.ProductCode = (byte)productCode;

            companyAttachedProduct.TargetProductId = targetProductId;

            companyAttachedProduct.CreatedDate = DateTime.Now;

            return companyAttachedProduct;
        }
    }
}
