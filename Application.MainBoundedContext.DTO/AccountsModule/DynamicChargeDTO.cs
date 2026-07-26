using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class DynamicChargeDTO : BindingModelBase<DynamicChargeDTO>
    {
        public DynamicChargeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string ExtendedDescription
        {
            get
            {
                return string.Format("{0} ({1} recovery from {2})", Description, RecoveryModeDescription, RecoverySourceDescription);
            }
        }

        [DataMember]
        [Display(Name = "Recovery Mode")]
        public int RecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Recovery Mode")]
        public string RecoveryModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DynamicChargeRecoveryMode), RecoveryMode) ? EnumHelper.GetDescription((DynamicChargeRecoveryMode)RecoveryMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Recovery Source")]
        public int RecoverySource { get; set; }

        [DataMember]
        [Display(Name = "Recovery Source")]
        public string RecoverySourceDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DynamicChargeRecoverySource), RecoverySource) ? EnumHelper.GetDescription((DynamicChargeRecoverySource)RecoverySource) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Installments Basis Value")]
        public int InstallmentsBasisValue { get; set; }

        [DataMember]
        [Display(Name = "Installments Basis Value")]
        public string InstallmentsBasisValueDescription
        {
            get
            {
                return Enum.IsDefined(typeof(DynamicChargeInstallmentsBasisValue), InstallmentsBasisValue) ? EnumHelper.GetDescription((DynamicChargeInstallmentsBasisValue)InstallmentsBasisValue) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Installments")]
        public int Installments { get; set; }

        [DataMember]
        [Display(Name = "Factor in Loan Term?")]
        public bool FactorInLoanTerm { get; set; }

        [DataMember]
        [Display(Name = "Compute charge on Top-up?")]
        public bool ComputeChargeOnTopUp { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
