using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Application.MainBoundedContext.DTO.AdministrationModule;
using System.Collections.ObjectModel;





namespace Application.MainBoundedContext.DTO.HumanResourcesModule
{
    public class SalaryProcessingDTO : BindingModelBase<SalaryProcessingDTO>
    {
        public int ModuleNavigationItemCode;
        public SalaryProcessingDTO()
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
                return Enum.IsDefined(typeof(Month), Month) ? EnumHelper.GetDescription((Month)Month) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Employee Category")]
        public int EmployeeCategory { get; set; }

        [DataMember]
        [Display(Name = "Employee Category")]
        public string EmployeeCategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(EmployeeCategory), EmployeeCategory) ? EnumHelper.GetDescription((EmployeeCategory)EmployeeCategory) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Tax Relief Amount")]
        public decimal TaxReliefAmount { get; set; }

        [DataMember]
        [Display(Name = "Maximum Provident Fund Relief Amount")]
        public decimal MaximumProvidentFundReliefAmount { get; set; }
        
        [DataMember]
        [Display(Name = "Maximum Insurance Relief Amount")]
        public decimal MaximumInsuranceReliefAmount { get; set; }

        [DataMember]
        [Display(Name = "Enforce Month Value Date?")]
        public bool EnforceMonthValueDate { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public int Status { get; set; }

        [DataMember]
        [Display(Name = "Status")]
        public string StatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryPeriodStatus), Status) ? EnumHelper.GetDescription((SalaryPeriodStatus)Status) : string.Empty;
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
        [Display(Name = "Execute Payout Standing Orders?")]
        public bool ExecutePayoutStandingOrders { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Total Net Pay")]
        public decimal TotalNetPay { get; set; }

        [DataMember]
        [Display(Name = "Posted Entries")]
        public string PostedEntries { get; set; }
        public ObservableCollection<SalaryGroupDTO> SelectedGroups { get; set; }
        public ObservableCollection<BranchDTO> SelectedBranches { get; set; }
        public ObservableCollection<DepartmentDTO> SelectedDepartments { get; set; }
        
        

        [DataMember]
        public string ErrorMessageResult { get; set; }
    }
}
