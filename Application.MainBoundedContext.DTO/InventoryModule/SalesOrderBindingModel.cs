using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class SalesOrderBindingModel : BindingModelBase<SalesOrderBindingModel>
    {
        public SalesOrderBindingModel()
        {
            AddAllAttributeValidators();
        }


        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Order Quantity")]
        public decimal OrderQuantity { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [DataMember]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }

        [DataMember]
        [Display(Name = "Customer Contact No.")]
        public string CustomerContact { get; set; }

        [DataMember]
        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; }


        [DataMember]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

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
        public List<SalesOrderEntryDTO> SalesOrderEntry { get; set; }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Inventory Description")]
        public string InventoryDescription { get; set; }
        public List<SalesOrderEntryBindingModel> SalesOrderEntries { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        
    }
}
