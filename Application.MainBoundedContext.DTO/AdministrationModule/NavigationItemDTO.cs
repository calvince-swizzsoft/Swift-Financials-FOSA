using Application.Seedwork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class NavigationItemDTO : BindingModelBase<NavigationItemDTO>
    {
        public NavigationItemDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public NavigationItemDTO Parent { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Icon")]
        public string Icon { get; set; }

        [DataMember]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [DataMember]
        [Display(Name = "Controller Name")]
        public string ControllerName { get; set; }

        [DataMember]
        [Display(Name = "Action Name")]
        public string ActionName { get; set; }

        [DataMember]
        [Display(Name = "Area Name")]
        public string AreaName { get; set; }

        [DataMember]
        [Display(Name = "Parent Code")]
        public int AreaCode { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Is Area?")]
        public bool IsArea { get; set; }

        //temp hack
        public List<NavigationItemDTO> Child { get; set; }

        HashSet<NavigationItemDTO> _children;
        [DataMember]
        [Display(Name = "Children")]
        public virtual ICollection<NavigationItemDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<NavigationItemDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<NavigationItemDTO>(value);
            }
        }
    }
}