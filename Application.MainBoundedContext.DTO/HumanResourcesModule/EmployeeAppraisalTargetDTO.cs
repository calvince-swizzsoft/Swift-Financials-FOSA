using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeAppraisalTargetDTO : BindingModelBase<EmployeeAppraisalTargetDTO>
    {
        public EmployeeAppraisalTargetDTO()
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
        public EmployeeAppraisalTargetDTO Parent { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeAppraisalTargetType), Type) ? EnumHelper.GetDescription((EmployeeAppraisalTargetType)Type) : string.Empty;
            }
        }

        [Display(Name = "Answer Type")]
        public int AnswerType { get; set; }

        [DataMember]
        [Display(Name = "Answer Type")]
        public string AnswerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeAppraisalTargetAnswerType), AnswerType) ? EnumHelper.GetDescription((EmployeeAppraisalTargetAnswerType)AnswerType) : string.Empty;
            }
        }

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

        HashSet<EmployeeAppraisalTargetDTO> _children;
        [DataMember]
        [Display(Name = "Children")]
        public virtual ICollection<EmployeeAppraisalTargetDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<EmployeeAppraisalTargetDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<EmployeeAppraisalTargetDTO>(value);
            }
        }
    }
}
