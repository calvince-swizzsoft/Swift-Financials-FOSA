
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class AlternateChannelDTO : BindingModelBase<AlternateChannelDTO>
    {
        public AlternateChannelDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Customer Account")]
        [ValidGuid]
        public Guid CustomerAccountId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerAccountCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerAccountCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerAccountCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerAccountCustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerAccountCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid CustomerAccountBranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int CustomerAccountBranchCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product")]
        public Guid CustomerAccountCustomerAccountTypeTargetProductId { get; set; }

        [DataMember]
        [Display(Name = "Product Code")]
        public int CustomerAccountCustomerAccountTypeTargetProductCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Product Code")]
        public int CustomerAccountCustomerAccountTypeProductCode { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Status")]
        public int CustomerAccountStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Status")]
        public string CustomerAccountStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerAccountStatus), CustomerAccountStatus) ? EnumHelper.GetDescription((CustomerAccountStatus)CustomerAccountStatus) : string.Empty;
            }
        }







        [DataMember]
        [Display(Name = "Operations")]
        public int Operations { get; set; }

        [DataMember]
        [Display(Name = "Operations")]
        public string OperationsDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AlternateChannelManagementAction), Operations) ? EnumHelper.GetDescription((AlternateChannelManagementAction)Operations) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Account Record Status")]
        public int CustomerAccountRecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Customer Account Record Status")]
        public string CustomerAccountRecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), CustomerAccountRecordStatus) ? EnumHelper.GetDescription((RecordStatus)CustomerAccountRecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer Account Remarks")]
        public string CustomerAccountRemarks { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerAccountCustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Full Account Number")]
        public string FullAccountNumber
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}",
                            CustomerAccountBranchCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerSerialNumber.ToString().PadLeft(7, '0'),
                            CustomerAccountCustomerAccountTypeProductCode.ToString().PadLeft(3, '0'),
                            CustomerAccountCustomerAccountTypeTargetProductCode.ToString().PadLeft(3, '0'));
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerAccountCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerAccountCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerAccountCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerAccountCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string CustomerAccountCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string CustomerAccountCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerAccountCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerAccountCustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerAccountCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerAccountCustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string CustomerAccountCustomerPersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerAccountCustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerAccountCustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerAccountCustomerIndividualSalutationDescription, CustomerAccountCustomerIndividualFirstName, CustomerAccountCustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerAccountCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAccountCustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAccountCustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerAccountCustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Is Defaulter?")]
        public bool CustomerAccountCustomerIsDefaulter { get; set; }

        [DataMember]
        [Display(Name = "Product Name")]
        public string ProductDescription { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account")]
        public Guid ProductChartOfAccountId { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Code")]
        public int ProductChartOfAccountCode { get; set; }

        [DataMember]
        [Display(Name = "Product G/L Account Name")]
        public string ProductChartOfAccountName { get; set; }

        [DataMember]
        [Display(Name = "Channel Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Channel Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(AlternateChannelType), Type) ? EnumHelper.GetDescription((AlternateChannelType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Primary Account Number")]
        [CustomValidation(typeof(AlternateChannelDTO), "CheckAlternateChannelNumber", ErrorMessage = "The Primary Account Number is not valid!")]
        public string CardNumber { get; set; }

        [DataMember]
        [Display(Name = "Primary Account Number")]
        public string MaskedCardNumber
        {
            get
            {
                var maskedPAN = string.Empty;

                switch ((AlternateChannelType)Type)
                {
                    case AlternateChannelType.Citius:
                    case AlternateChannelType.AgencyBanking:
                    case AlternateChannelType.MCoopCash:
                    case AlternateChannelType.SpotCash:
                    case AlternateChannelType.PesaPepe:
                    case AlternateChannelType.Broker:
                        maskedPAN = UberUtil.MaskPan(CardNumber, 4, 3);
                        break;
                    case AlternateChannelType.Sparrow:
                    case AlternateChannelType.SaccoLink:
                    case AlternateChannelType.AbcBank:
                    default:
                        maskedPAN = UberUtil.MaskPan(CardNumber);
                        break;
                }

                return maskedPAN;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Third Party Notified?")]
        public bool IsThirdPartyNotified { get; set; }

        [DataMember]
        [Display(Name = "Third Party Response")]
        public string ThirdPartyResponse { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Valid From")]
        public DateTime ValidFrom { get; set; }

        [DataMember]
        [Display(Name = "Expires")]
        public DateTime Expires { get; set; }

        [DataMember]
        [Display(Name = "Daily Limit")]
        public decimal DailyLimit { get; set; }

        [DataMember]
        [Display(Name = "Mobile PIN")]
        public string MobilePIN { get; set; }

        [DataMember]
        [Display(Name = "New Mobile PIN")]
        public string NewMobilePIN { get; set; }

        [DataMember]
        [Display(Name = "Reset Mobile PIN")]
        public bool ResetMobilePIN { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(RecordStatus), RecordStatus) ? EnumHelper.GetDescription((RecordStatus)RecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [DataMember]
        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [DataMember]
        [Display(Name = "Recruited By")]
        public string RecruitedBy { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string ErrorMessageResult { get; set; }
        public static ValidationResult CheckAlternateChannelNumber(string value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as AlternateChannelDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be AlternateChannelDTO");

            if (string.IsNullOrEmpty(value))
                return new ValidationResult(string.Format("The Primary Account Number {0} is not valid!", value));
            else
            {
                var temp = Regex.Replace(value, "[^0-9.]", "");

                switch ((AlternateChannelType)bindingModel.Type)
                {
                    case AlternateChannelType.SaccoLink:
                    case AlternateChannelType.AbcBank:
                        bindingModel.CardNumber = UberUtil.IsValidPAN(temp) ? temp : string.Empty;
                        break;
                    case AlternateChannelType.MCoopCash:
                    case AlternateChannelType.SpotCash:
                    case AlternateChannelType.PesaPepe:
                        bindingModel.CardNumber = Regex.IsMatch(string.Format("+{0}", temp), @"^\+(?:[0-9]??){6,14}[0-9]$") ? temp : string.Empty;
                        break;
                    case AlternateChannelType.Sparrow:
                        bindingModel.CardNumber = (temp.Length == 13) ? temp : string.Empty;
                        break;
                    case AlternateChannelType.AgencyBanking:
                    case AlternateChannelType.Citius:
                    default:
                        bindingModel.CardNumber = string.Empty;
                        break;
                }

                if (string.IsNullOrEmpty(bindingModel.CardNumber))
                    return new ValidationResult(string.Format("The Primary Account Number {0} is not valid!", value));
                else return ValidationResult.Success;
            }
        }
    }
}
