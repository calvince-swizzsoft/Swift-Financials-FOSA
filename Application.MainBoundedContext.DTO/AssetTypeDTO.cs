using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class AssetTypeDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Depreciation Method")]
        public string DepreciationMethod { get; set; }

        [DataMember]
        [Display(Name = "Useful Life (Years)")]
        public int UsefulLife { get; set; }

        [DataMember]
        [Display(Name = "Is Tangible")]
        public bool IsTangible { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}

