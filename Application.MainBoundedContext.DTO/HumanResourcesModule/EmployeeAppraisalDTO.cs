using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class EmployeeAppraisalDTO : BindingModelBase<EmployeeAppraisalDTO>
    {
        public EmployeeAppraisalDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Period")]
        [ValidGuid]
        public Guid EmployeeAppraisalPeriodId { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Period")]
        public string EmployeeAppraisalPeriodDescription { get; set; }

        [DataMember]
        [Display(Name = "Appraisal Start Date")]
        public DateTime EmployeeAppraisalPeriodDurationStartDate { get; set; }

        [DataMember]
        [Display(Name = "Appraisal End Date")]
        public DateTime EmployeeAppraisalPeriodDurationEndDate { get; set; }

        [DataMember]
        [Display(Name = "Employee")]
        [ValidGuid]
        public Guid EmployeeId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Target")]
        [ValidGuid]
        public Guid EmployeeAppraisalTargetId { get; set; }

        [DataMember]
        [Display(Name = "Target")]
        public string EmployeeAppraisalTargetDescription { get; set; }

        [Display(Name = "Answer Type")]
        public int EmployeeAppraisalTargetAnswerType { get; set; }

        [DataMember]
        [Display(Name = "Answer Type")]
        public string AnswerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeAppraisalTargetAnswerType), EmployeeAppraisalTargetAnswerType) ? EnumHelper.GetDescription((EmployeeAppraisalTargetAnswerType)EmployeeAppraisalTargetAnswerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Appraisee Answer")]
        public string AppraiseeAnswer { get; set; }

        [DataMember]
        [Display(Name = "Appraisee Answer")]
        public int AppraiseeAnswerRating
        {
            get
            {
                int numericAppraiseeAnswer;
                bool isNumeric = int.TryParse(AppraiseeAnswer, out numericAppraiseeAnswer);
                return numericAppraiseeAnswer;
            }
        }

        [DataMember]
        [Display(Name = "Appraiser Answer")]
        public string AppraiserAnswer { get; set; }

        [DataMember]
        [Display(Name = "Appraiser Answer")]
        public int AppraiserAnswerRating
        {
            get
            {
                int numericAppraiseeAnswer;
                bool isNumeric = int.TryParse(AppraiserAnswer, out numericAppraiseeAnswer);
                return numericAppraiseeAnswer;
            }
        }

        [DataMember]
        [Display(Name = "Appraised By")]
        public string AppraisedBy { get; set; }

        [DataMember]
        [Display(Name = "Appraised Date")]
        public DateTime? AppraisedDate { get; set; }

        [DataMember]
        [Display(Name = "Self Appraisal Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }
    }
}
