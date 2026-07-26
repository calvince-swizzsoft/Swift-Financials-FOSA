using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class DesignationDTO : BindingModelBase<DesignationDTO>
    {
        public DesignationDTO()
        {
            AddAllAttributeValidators();
            TransactionThresholds = new ObservableCollection<TransactionThresholdDTO>();


        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public DesignationDTO Parent { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public string IndentedDescription
        {
            get
            {
                var tabs = string.Empty;

                for (int i = 0; i < Depth; i++)
                {
                    tabs += "\t";
                }

                return string.Format("{0}{1}-{2}", tabs, Depth, Description);
            }
        }

        HashSet<DesignationDTO> _children;
        [DataMember]
        [Display(Name = "Children")]
        public virtual ICollection<DesignationDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<DesignationDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<DesignationDTO>(value);
            }
        }

        [DataMember]
        [Display(Name = "Type")]
        public short Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((SystemTransactionCode)Type);
            }
        }

        [DataMember]
        [Display(Name = "Threshold")]
        public decimal Threshold { get; set; }

        [DataMember]
        [Display(Name = "UserName")]
        public string ActiveUser { get; set; }

        public ObservableCollection<TransactionThresholdDTO> TransactionThresholds { get; set; }


    }

   

}