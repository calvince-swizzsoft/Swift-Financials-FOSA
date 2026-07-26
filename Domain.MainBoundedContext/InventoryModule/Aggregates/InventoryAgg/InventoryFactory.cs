using Domain.MainBoundedContext.AdministrationModule.Aggregates.CompanyAgg;
using System;

namespace Domain.MainBoundedContext.InventoryModule.Aggregates.InventoryAgg
{
    public static class InventoryFactory
    {
        public static Inventory CreateInventory(
            string code,
            Guid companyId,
            Guid categoryId,
            string description,
            decimal quantityInstore,
            string remarks,
            decimal unitPrice,
            string unitOfMeasure,
            int status,
            DateTime expiryDate,
            byte[] image)  
        {
            var inventory = new Inventory();
            inventory.GenerateNewIdentity();

            inventory.Code = code;
            inventory.CompanyId = companyId;
            inventory.CategoryId = categoryId;
            inventory.Description = description;
            inventory.Remarks = remarks;
            inventory.QuantityInstore = quantityInstore;
            inventory.UnitPrice = unitPrice;
            inventory.UnitOfMeasure = unitOfMeasure;
            inventory.Status = status;
            inventory.ExpiryDate = expiryDate;
            inventory.CreatedDate = DateTime.Now;
            inventory.Image = image;  

            return inventory;
        }
    }
}
