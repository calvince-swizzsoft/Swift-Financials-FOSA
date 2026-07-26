
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SystemGeneralLedgerAccountMappingDTO : BindingModelBase<SystemGeneralLedgerAccountMappingDTO>
    {
        public SystemGeneralLedgerAccountMappingDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "System G/L Account Code")]
        public int SystemGeneralLedgerAccountCode { get; set; }

        [DataMember]
        [Display(Name = "System G/L Account Code")]
        public string SystemGeneralLedgerAccountCodeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(SystemGeneralLedgerAccountCode), SystemGeneralLedgerAccountCode) ? EnumHelper.GetDescription((SystemGeneralLedgerAccountCode)SystemGeneralLedgerAccountCode) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Mapped G/L Account")]
        [ValidGuid]
        public Guid ChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Mapped G/L Account Type")]
        public int ChartOfAccountAccountType { get; set; }

        [DataMember]
        [Display(Name = "Mapped G/L Account Code")]
        public int ChartOfAccountAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Mapped G/L Account Name")]
        public string ChartOfAccountAccountName { get; set; }

        [DataMember]
        [Display(Name = "Mapped G/L Account Name")]
        public string ChartOfAccountName
        {
            get
            {
                return string.Format("{0}-{1} {2}", ChartOfAccountAccountType.FirstDigit(), ChartOfAccountAccountCode, ChartOfAccountAccountName);
            }
        }

        [DataMember]
        [Display(Name = "Mapped G/L Account Cost Center")]
        public Guid? ChartOfAccountCostCenterId { get; set; }

        [DataMember]
        [Display(Name = "Mapped G/L Account Cost Center")]
        public string ChartOfAccountCostCenterDescription { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        //[DataMember]
        //[Display(Name = "Name")]
        //[Required]
        //public string Description { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }


        [DataMember]
        public string ErrorMessageResult { get; set; }
    }
}
