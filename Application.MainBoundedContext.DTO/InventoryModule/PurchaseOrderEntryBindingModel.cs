using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class PurchaseOrderEntryBindingModel : BindingModelBase<PurchaseOrderEntryBindingModel>
    {
        public PurchaseOrderEntryBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Purchase Order")]
        public Guid PurchaseOrderId { get; set; }

        [DataMember]
        [Display(Name = "Inventory")]
        public Guid InventoryId { get; set; }

        [DataMember]
        [Display(Name = "Item Description")]
        public string InventoryDescription { get; set; }

        [DataMember]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [DataMember]
        [Display(Name = "Amount Per Unit")]
        public decimal AmountPerUnit { get; set; }

        [DataMember]
        [Display(Name = "Purchased Quantity")]
        public int OrderQuantity { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
