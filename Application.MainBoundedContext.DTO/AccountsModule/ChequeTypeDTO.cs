using Application.Seedwork;

using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ChequeTypeDTO : BindingModelBase<ChequeTypeDTO>
    {
        public ChequeTypeDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Cheque")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Maturity Period")]
        public int MaturityPeriod { get; set; }

        [Display(Name = "Charge Recovery Mode")]
        public int ChargeRecoveryMode { get; set; }

        [DataMember]
        [Display(Name = "Charge Recovery Mode")]
        public string ChargeRecoveryModeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChequeTypeChargeRecoveryMode), ChargeRecoveryMode) ? EnumHelper.GetDescription((ChequeTypeChargeRecoveryMode)ChargeRecoveryMode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
