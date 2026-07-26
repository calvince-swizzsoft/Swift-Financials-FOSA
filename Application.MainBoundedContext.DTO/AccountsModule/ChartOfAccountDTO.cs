using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ChartOfAccountDTO : BindingModelBase<ChartOfAccountDTO>
    {
        public ChartOfAccountDTO()
        {
            AddAllAttributeValidators();
        }

       
        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent G/L Acccount")]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public ChartOfAccountDTO Parent { get; set; }

        [Display(Name = "Parent Account Name")]
        public string ParentAccountName { get; set; }

        [DataMember]
        [Display(Name = "Cost Center")]
        public Guid? CostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Cost Center")]
        public string CostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Account Type")]
       // [CustomValidation(typeof(ChartOfAccountDTO), "CheckAccountType")]
        public int AccountType { get; set; }

        [DataMember]
        [Display(Name = "Account Type")]
        public string AccountTypeDescription => Enum.IsDefined(typeof(ChartOfAccountType), AccountType) ? EnumHelper.GetDescription((ChartOfAccountType)AccountType) : string.Empty;

        [DataMember]
        [Display(Name = "Account Category")]
      //  [CustomValidation(typeof(ChartOfAccountDTO), "CheckAccountCategory")]
        public int AccountCategory { get; set; }

        [DataMember]
        [Display(Name = "Account Category")]
        public string AccountCategoryDescription => Enum.IsDefined(typeof(ChartOfAccountCategory), AccountCategory) ? EnumHelper.GetDescription((ChartOfAccountCategory)AccountCategory) : string.Empty;

        [DataMember]
        [Display(Name = "Account Code")]
        public int AccountCode { get; set; }

        [DataMember]
        [Display(Name = "Account Name")]
        [Required]
        public string AccountName { get; set; }

        [DataMember]
        [Display(Name = "Depth")]
        public int Depth { get; set; }

        [DataMember]
        [Display(Name = "Is Control Account?")]
        public bool IsControlAccount { get; set; }

        [DataMember]
        [Display(Name = "Is Control Account?")]
        public string IsControlAccountDescription
        {
            get
            {
                return IsControlAccount ? "Yes" : "No";
            }
        }

        [DataMember]
        [Display(Name = "Is Reconciliation Account?")]
        public bool IsReconciliationAccount { get; set; }

        [DataMember]
        [Display(Name = "Is Reconciliation Account?")]
        public string IsReconciliationAccountDescription
        {
            get
            {
                return IsReconciliationAccount ? "Yes" : "No";
            }
        }

        [DataMember]
        [Display(Name = "Post Automatically Only?")]
        public bool PostAutomaticallyOnly { get; set; }

        [DataMember]
        [Display(Name = "Post Automatically Only?")]
        public string PostAutomaticallyOnlyDescription
        {
            get
            {
                return PostAutomaticallyOnly ? "Yes" : "No";
            }
        }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public string IsLockedDescription
        {
            get
            {
                return IsLocked ? "Yes" : "No";
            }
        }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        HashSet<ChartOfAccountDTO> _children;
        [DataMember]
        [Display(Name = "Children")]
        public virtual ICollection<ChartOfAccountDTO> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new HashSet<ChartOfAccountDTO>();
                }
                return _children;
            }
            private set
            {
                _children = new HashSet<ChartOfAccountDTO>(value);
            }
        }

        public static ValidationResult CheckAccountType(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as ChartOfAccountDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be ChartOfAccountDTO");

            ChartOfAccountType accountType = ChartOfAccountType.Asset;
            if (string.IsNullOrWhiteSpace(value) || !Enum.TryParse<ChartOfAccountType>(value, out accountType))
                return new ValidationResult("UnKnown Account Type.");

            return ValidationResult.Success;
        }

        public static ValidationResult CheckAccountCategory(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as ChartOfAccountDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be ChartOfAccountDTO");

            ChartOfAccountCategory accountCategory = ChartOfAccountCategory.DetailAccount;
            if (string.IsNullOrWhiteSpace(value) || !Enum.TryParse<ChartOfAccountCategory>(value, out accountCategory))
                return new ValidationResult("UnKnown Account Category.");

            return ValidationResult.Success;
        }

        [DataMember]
        public string ErrorMessageResult { get; set; }

    }
}
