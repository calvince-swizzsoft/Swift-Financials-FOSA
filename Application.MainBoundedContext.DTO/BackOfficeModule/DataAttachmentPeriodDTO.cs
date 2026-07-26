
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

namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
    public class DataAttachmentPeriodDTO : BindingModelBase<DataAttachmentPeriodDTO>
    {
        public DataAttachmentPeriodDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        [ValidGuid]
        public Guid PostingPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Posting Period")]
        public string PostingPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public int Month { get; set; }

        [DataMember]
        [Display(Name = "Month")]
        public string MonthDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Month), Month) ? string.Format("{0} - {1}", EnumHelper.GetDescription((Month)Month), PostingPeriodDescription) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DataAttachmentPeriodStatus), Status) ? EnumHelper.GetDescription((DataAttachmentPeriodStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }



        // Additional DTOs
        // Additional DTOs
        [DataMember]
        [Display(Name = "Data Period")]
        public string DataAttachmentPeriodDescription { get; set; }

        [DataMember]
        public string ErrorMessageResult { get; set; }
    }
}
