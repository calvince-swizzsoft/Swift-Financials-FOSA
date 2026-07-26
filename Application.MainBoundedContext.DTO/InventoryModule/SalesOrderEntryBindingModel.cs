using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
   public class SalesOrderEntryBindingModel : BindingModelBase<SalesOrderEntryBindingModel>
    {
        public SalesOrderEntryBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Sales Order Id")]
        public Guid SalesOrderId { get; set; }

        [DataMember]
        [Display(Name = "Inventory Id")]
        public Guid InventoryId { get; set; }

        [DataMember]
        [Display(Name = "Inventory Description")]
        public string InventoryDescription { get; set; }

        [DataMember]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [DataMember]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [DataMember]
        [Display(Name = "Order Quantity")]
        public decimal OrderQuantity { get; set; }

        [DataMember]
        [Display(Name = "Order Details")]
        public List<SalesOrderEntryDTO> SalesOrderEntry { get; set; }
    }
}
