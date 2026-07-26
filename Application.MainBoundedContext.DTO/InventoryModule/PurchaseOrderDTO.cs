using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class PurchaseOrderDTO
    {
        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Purchased Quantity")]
        public decimal OrderQuantity { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [DataMember]
        [Display(Name = "Supplier Contact.")]
        public string SupplierContact { get; set; }

        [DataMember]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Item Description")]
        public string InventoryDescription { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public short RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription => EnumHelper.GetDescription((PurchaseOrderStatus)RecordStatus);

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Order Details")]
        public List<PurchaseOrderEntryDTO> PurchaseOrderEntry { get; set; }
    }
}
