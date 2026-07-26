using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class InventoryBindingModel : BindingModelBase<InventoryBindingModel>
    {
        public InventoryBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        [Required]
        public string Code { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Organisation")]
        public Guid CompanyId { get; set; }

        [DataMember]
        [Display(Name = "Organisation")]
        public string CompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string CategoryDescription { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription => EnumHelper.GetDescription((InventoryRecordStatus)Status);

        [DataMember]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [DataMember]
        [Display(Name = "Quantity In Store")]
        public decimal QuantityInStore { get; set; }

        [DataMember]
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Product Image")]
        public byte[] Image { get; set; }
    }
}
