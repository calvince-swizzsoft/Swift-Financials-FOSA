using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.InventoryModule
{
    public class AssetTypeBindingModel : BindingModelBase<AssetTypeBindingModel>
    {
        public AssetTypeBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Depreciation Method")]
        [Required]
        public int DepreciationMethod { get; set; }

        [DataMember]
        [Display(Name = "Useful Life (Years)")]
        [Range(1, int.MaxValue, ErrorMessage = "Useful life must be at least 1 year.")]
        public int UsefulLife { get; set; }

        [DataMember]
        [Display(Name = "Is Tangible")]
        public bool IsTangible { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
