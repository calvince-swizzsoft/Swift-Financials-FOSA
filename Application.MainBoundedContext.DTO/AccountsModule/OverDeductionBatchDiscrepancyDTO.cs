using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class OverDeductionBatchDiscrepancyDTO : BindingModelBase<OverDeductionBatchDiscrepancyDTO>
    {
        public OverDeductionBatchDiscrepancyDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Over Deduction Batch")]
        [ValidGuid]
        public Guid OverDeductionBatchId { get; set; }

        [DataMember]
        [Display(Name = "Column 1")]
        public string Column1 { get; set; }

        [DataMember]
        [Display(Name = "Column 2")]
        public string Column2 { get; set; }

        [DataMember]
        [Display(Name = "Column 3")]
        public string Column3 { get; set; }

        [DataMember]
        [Display(Name = "Column 4")]
        public string Column4 { get; set; }

        [DataMember]
        [Display(Name = "Column 5")]
        public string Column5 { get; set; }

        [DataMember]
        [Display(Name = "Column 6")]
        public string Column6 { get; set; }

        [DataMember]
        [Display(Name = "Column 7")]
        public string Column7 { get; set; }

        [DataMember]
        [Display(Name = "Column 8")]
        public string Column8 { get; set; }

        [DataMember]
        [Display(Name = "Column 9")]
        public string Column9 { get; set; }

        [DataMember]
        [Display(Name = "Column 10")]
        public string Column10 { get; set; }

        [DataMember]
        [Display(Name = "Column 11")]
        public string Column11 { get; set; }

        [DataMember]
        [Display(Name = "Column 12")]
        public string Column12 { get; set; } 

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(BatchEntryStatus), Status) ? EnumHelper.GetDescription((BatchEntryStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Posted By")]
        public string PostedBy { get; set; }

        [DataMember]
        [Display(Name = "Posted Date")]
        public DateTime? PostedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
