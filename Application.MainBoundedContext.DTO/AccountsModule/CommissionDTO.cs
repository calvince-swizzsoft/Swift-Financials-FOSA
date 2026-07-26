using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class CommissionDTO : BindingModelBase<CommissionDTO>
    {
        public CommissionDTO commissionDTO;

        public CommissionDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }


        [DataMember]
        [Display(Name = "Charge")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Range (Lower Limit)")]
        public decimal LowerLimit { get; set; }

        [DataMember]
        [Display(Name = "Range (Upper Limit)")]
        public decimal UpperLimit { get; set; }

        [DataMember]
        [Display(Name = "Maximum Charge")]
        public decimal MaximumCharge { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public int RoundingType { get; set; }

        [DataMember]
        [Display(Name = "Rounding Type")]
        public string RoundingTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RoundingType), RoundingType) ? EnumHelper.GetDescription((RoundingType)RoundingType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }


        [DataMember]
        [Display(Name = "Excise Duty")]
        public bool ExciseDuty { get; set; }


        [DataMember]
        [Display(Name = "Withholding Tax")]
        public bool WithHoldingTax { get; set; }


        [DataMember]
        [Display(Name = "School")]
        public bool School { get; set; }


        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
               
        [DataMember]
        [Display(Name = "Leviable?")]
        public bool Leviable { get; set; }

        [DataMember]
        [Display(Name = "Charge Benefactor")]
        public int ChargeBenefactor { get; set; }

        [DataMember]
        [Display(Name = "Charge Benefactor")]
        public string ChargeBenefactorDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeBenefactor), ChargeBenefactor) ? EnumHelper.GetDescription((ChargeBenefactor)ChargeBenefactor) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Charge Basis Value")]
        public int ChargeBasisValue { get; set; }

        [DataMember]
        [Display(Name = "Charge Basis Value")]
        public string ChargeBasisValueDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductChargeBasisValue), ChargeBasisValue) ? EnumHelper.GetDescription((LoanProductChargeBasisValue)ChargeBasisValue) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Known ChargeType")]
        public int KnownChargeType { get; set; }

        [DataMember]
        [Display(Name = "Known ChargeType")]
        public string KnownChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(LoanProductKnownChargeType), KnownChargeType) ? EnumHelper.GetDescription((LoanProductKnownChargeType)KnownChargeType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "System Transaction Type")]
        public int SystemTransactionType { get; set; }

        [DataMember]
        [Display(Name = "Complement Type")]
        public int ComplementType { get; set; }

        [DataMember]
        [Display(Name = "Complement Type")]
        public string ComplementTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ComplementType) ? EnumHelper.GetDescription((ChargeType)ComplementType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Complement Percentage")]
        public double ComplementPercentage { get; set; }

        [DataMember]
        [Display(Name = "Complement Fixed Amount")]
        public decimal ComplementFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string ExtendedChargeBenefactorDescription
        {
            get
            {
                return string.Format("{0} (borne by {1})", Description, ChargeBenefactorDescription);
            }
        }

        [DataMember]
        [Display(Name = "Name")]
        public string ExtendedChargeBasisValueDescription
        {
            get
            {
                return string.Format("{0} (basis value is {1})", Description, ChargeBasisValueDescription);
            }
        }

        public IList<LevyDTO> Levies { get; set; }

        public IList<LevySplitDTO> LevySplits { get; set; }

        public Guid CommissionSplitChartOfAccountId { get; set; }

        public CommissionSplitDTO CommissionSplit { get; set; }

        public CommissionLevyDTO CommissionLevy { get; set; }

        public IList<CommissionLevyDTO> CommissionLevies { get; set; }

        public IList<CommissionSplitDTO> CommissionSplits { get; set; }

        public static implicit operator ObservableCollection<object>(CommissionDTO v)
        {
            throw new NotImplementedException();
        }


        public CommissionSplitDTO chargeSplits { get; set; }
        public ChartOfAccountDTO chartOfAccount { get; set; }

        public ObservableCollection<CommissionSplitDTO> chargeSplit { get; set; }


        [DataMember]
        public string ErrorMessageResult { get; set; }


        [DataMember]
        public CustomerDTO CustomerDTO { get; set; }

        public ObservableCollection<GraduatedScaleDTO> fixedeposite { get; set; }

        [DataMember]
        [Display(Name = "G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }
    }
}
