using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class AdministrativeDivisionDTO
    { 
        [Display(Name = "Id")]
        public Guid Id { get; set; }
        
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }
        
        [Display(Name = "Parent")]
        public AdministrativeDivisionDTO Parent { get; set; }
        
        [Display(Name = "Name")]
        public string Description { get; set; }
        
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [Display(Name = "Type")]
        public byte Type { get; set; }
        
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((AdministrativeDivisionType)Type);
            }
        }
        
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }
        
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Description")]
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

        HashSet<AdministrativeDivisionDTO> _children;
        [Display(Name = "Children")]
        public virtual ICollection<AdministrativeDivisionDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<AdministrativeDivisionDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<AdministrativeDivisionDTO>(value);
            }
        }
    }
}
