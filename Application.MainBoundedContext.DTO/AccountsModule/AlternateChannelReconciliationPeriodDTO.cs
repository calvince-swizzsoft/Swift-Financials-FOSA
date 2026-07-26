using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AlternateChannelReconciliationPeriodDTO : BindingModelBase<AlternateChannelReconciliationPeriodDTO>
    {
        public AlternateChannelReconciliationPeriodDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Type")]
        public int AlternateChannelType { get; set; }

        [DataMember]
        [Display(Name = "Alternate Channel Type")]
        public string AlternateChannelTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AlternateChannelType), AlternateChannelType) ? EnumHelper.GetDescription((AlternateChannelType)AlternateChannelType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "End Date")]
        public DateTime DurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Set Difference Mode")]
        public int SetDifferenceMode { get; set; }

        [DataMember]
        [Display(Name = "Set Difference Mode")]
        public string SetDifferenceModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SetDifferenceMode), SetDifferenceMode) ? EnumHelper.GetDescription((SetDifferenceMode)SetDifferenceMode) : string.Empty;
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
                return Enum.IsDefined(typeof(AlternateChannelReconciliationPeriodStatus), Status) ? EnumHelper.GetDescription((AlternateChannelReconciliationPeriodStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejected By")]
        public string AuthorizedBy { get; set; }

        [DataMember]
        [Display(Name = "Authorization/Rejection Remarks")]
        public string AuthorizationRemarks { get; set; }

        [DataMember]
        [Display(Name = "Authorized/Rejection Date")]
        public DateTime? AuthorizedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public static ValidationResult CheckDurationEndDate(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as AlternateChannelReconciliationPeriodDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be AlternateChannelReconciliationPeriodDTO");

            if (bindingModel.DurationEndDate == null || bindingModel.DurationStartDate == null)
                return new ValidationResult("The duration dates must be specified.");
            else if (bindingModel.DurationEndDate <= bindingModel.DurationStartDate)
                return new ValidationResult("The end date must be greater than the start date.");

            return ValidationResult.Success;
        }
        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        public AlternateChannelReconciliationEntryDTO AlternateChannelReconciliationEntryDTO;
    }
}
