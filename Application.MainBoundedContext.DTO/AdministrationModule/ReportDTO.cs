
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class ReportDTO : BindingModelBase<ReportDTO>
    {
        public ReportDTO()
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
        public ReportDTO Parent { get; set; }

        [DataMember]
        [Display(Name = "Report Name")]
        [Required]
        public string ReportName { get; set; }

        [Display(Name = "Report Name")]
        public string IndentedReportName
        {
            get
            {
                var tabs = string.Empty;

                for (int i = 0; i < Depth; i++)
                {
                    tabs += "\t";
                }

                return string.Format("{0}{1}", tabs, ReportName);
            }
        }

        [DataMember]
        [Display(Name = "Report Path")]
        public string ReportPath { get; set; }

        [DataMember]
        [Display(Name = "Report Query")]
        public string ReportQuery { get; set; }

        [DataMember]
        [Display(Name = "SSRS Host")]
        public string ReportHost
        {
            get
            {
                return DefaultSettings.Instance.SSRSHost ?? "localhost";
            }
        }

        [DataMember]
        [Display(Name = "SSRS Port")]
        public int ReportPort
        {
            get
            {
                return DefaultSettings.Instance.SSRSPort ?? 80;
            }
        }

        [DataMember]
        [Display(Name = "Report URL")]
        public string ReportURL
        {
            get
            {
                if (Category == (int)ReportTemplateCategory.DetailEntry)
                {
                    var uriBuilder = new UriBuilder();

                    uriBuilder.Host = ReportHost;
                    uriBuilder.Port = ReportPort;
                    uriBuilder.Path = ReportPath;
                    uriBuilder.Query = ReportQuery;
                    uriBuilder.Scheme = "http";

                    return string.Format("{0}", uriBuilder);
                }
                else return string.Empty;
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

        [DataMember]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        HashSet<ReportDTO> _children;
        [DataMember]
        [Display(Name = "Children")]
        public virtual ICollection<ReportDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<ReportDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<ReportDTO>(value);
            }
        }
    }
}
