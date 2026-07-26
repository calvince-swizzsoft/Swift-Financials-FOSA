using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class AdministrativeDivisionBindingModel : BindingModelBase<AdministrativeDivisionBindingModel>
    {
        public AdministrativeDivisionBindingModel()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        [ValidGuid]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public AdministrativeDivisionDTO Parent { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public byte Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((AdministrativeDivisionType)Type);
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
    }
}
