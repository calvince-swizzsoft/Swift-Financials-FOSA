
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ReportTemplateDTO : BindingModelBase<ReportTemplateDTO>
    {
        public ReportTemplateDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent Template")]
        [ValidGuid]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public ReportTemplateDTO Parent { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Name")]
        public string IndentedDescription
        {
            get
            {
                var tabs = string.Empty;

                for (int i = 0; i < Depth; i++)
                {
                    tabs += "\t";
                }

                return string.Format("{0}{1}", tabs, Description);
            }
        }

        [Display(Name = "Category")]
        public int Category { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string CategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ReportTemplateCategory), Category) ? EnumHelper.GetDescription((ReportTemplateCategory)Category) : string.Empty;
            }
        }

        [Display(Name = "Spreadsheet Cell Reference")]
        public string SpreadsheetCellReference { get; set; }

        [Display(Name = "Book Balance")]
        public decimal BookBalance { get; set; }

        [DataMember]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [DataMember]
        [Display(Name = "Template File Name")]
        public string TemplateFileName { get; set; }

        [DataMember]    
        [Display(Name = "Template Cut-Off Date")]
        public DateTime TemplateCutOffDate { get; set; }

        [DataMember]
        [Display(Name = "Transaction Date Filter")]
        public int TransactionDateFilter { get; set; }

        [DataMember]
        [Display(Name = "Template File Buffer")]
        public byte[] TemplateFileBuffer { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        HashSet<ReportTemplateDTO> _children;
        [DataMember]
        [Display(Name = "Children")]
        public virtual ICollection<ReportTemplateDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<ReportTemplateDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<ReportTemplateDTO>(value);
            }
        }
    }
}
