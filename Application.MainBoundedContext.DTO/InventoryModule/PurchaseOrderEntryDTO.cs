using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class PurchaseOrderEntryDTO: BindingModelBase<PurchaseOrderEntryDTO>
    {
        public PurchaseOrderEntryDTO()
        {
            AddAllAttributeValidators();
        }

        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Purchase Order")]
        public Guid PurchaseOrderId { get; set; }

        [Display(Name = "Inventory")]
        public Guid InventoryId { get; set; }

        [Display(Name = "Item Description")]
        public string InventoryDescription { get; set; }

        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Amount Per Unit")]
        public decimal AmountPerUnit { get; set; }

        [DataMember]
        [Display(Name = "Purchased Quantity")]
        public decimal OrderQuantity { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
