using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class WithdrawalNotificationDTO : BindingModelBase<WithdrawalNotificationDTO>
    {
        public WithdrawalNotificationDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Member Name")]
        //[ValidGuid]
        public Guid CustomerId { get; set; }


        [DataMember]
        [Display(Name = "Customer")]
        public CustomerDTO Customer { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int CustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string CustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), CustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)CustomerIndividualSalutation) : string.Empty;
            }
        }


        [DataMember]
        [Display(Name = "verify")]
        public int Verify { get; set; }

        [DataMember]
        [Display(Name = "verify")]
        public string verifyDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MembershipWithdrawalAuditOption), Verify) ? EnumHelper.GetDescription((MembershipWithdrawalAuditOption)Verify) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public int CustomerIndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string CustomerIndividualNationalityDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Nationality), CustomerIndividualNationality) ? EnumHelper.GetDescription((Nationality)CustomerIndividualNationality) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int CustomerSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedCustomerSerialNumber
        {
            get
            {
                return string.Format("{0}", CustomerSerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string CustomerFullName { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public int CustomerIndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string CustomerIndividualGenderDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Gender), CustomerIndividualGender) ? EnumHelper.GetDescription((Gender)CustomerIndividualGender) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public int CustomerIndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string CustomerIndividualMaritalStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MaritalStatus), CustomerIndividualMaritalStatus) ? EnumHelper.GetDescription((MaritalStatus)CustomerIndividualMaritalStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string CustomerAddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string CustomerAddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string CustomerAddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string CustomerAddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string CustomerAddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string CustomerAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string CustomerAddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string CustomerStationDescription { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        public string CustomerStationZoneDescription { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public string CustomerStationZoneDivisionDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string CustomerStationZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }

        [DataMember]
        [Display(Name = "Branch")]

        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch Code")]
        public int BranchCode { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Transfer net refundable amount to savings A/C on death claim settlement?")]
        public bool BranchCompanyTransferNetRefundableAmountToSavingsAccountOnDeathClaimSettlement { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public int Category { get; set; }

        [DataMember]
        [Display(Name = "Category")]
        public string CategoryDescription
        {
            get
            {
                return Enum.IsDefined(typeof(WithdrawalNotificationCategory), Category) ? EnumHelper.GetDescription((WithdrawalNotificationCategory)Category) : string.Empty;
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
                return Enum.IsDefined(typeof(WithdrawalNotificationStatus), Status) ? EnumHelper.GetDescription((WithdrawalNotificationStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Maturity Date")]
        public DateTime MaturityDate { get; set; }

        [DataMember]
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [DataMember]
        [Display(Name = "Approval Remarks")]
        public string ApprovalRemarks { get; set; }

        [DataMember]
        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        [DataMember]
        [Display(Name = "Verified By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verified Date")]
        public DateTime? AuditedDate { get; set; }

        [DataMember]
        [Display(Name = "Verification Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Settled By")]
        public string SettledBy { get; set; }

        [DataMember]
        [Display(Name = "Settled Date")]
        public DateTime? SettledDate { get; set; }

        [DataMember]
        [Display(Name = "Settlement Remarks")]
        public string SettlementRemarks { get; set; }

        [DataMember]
        [Display(Name = "Settlement Type")]
        public int SettlementType { get; set; }

        [DataMember]
        [Display(Name = "Settlement Type")]
        public string SettlementTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MembershipWithdrawalSettlementType), SettlementType) ? EnumHelper.GetDescription((MembershipWithdrawalSettlementType)SettlementType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ErrorMessageResult;
        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
       
        public WithdrawalSettlementDTO withdrawalSettlementDTO { get; set; }
        [DataMember]
        [Display(Name = "Net Refundable")]
        public decimal NetRefundable { get; set; }

        [DataMember]
        [Display(Name = "Net Refundable Validation")]
        //[CustomValidation(typeof(WithdrawalNotificationDTO), "ValidateNetRefundable", ErrorMessage = "The net refundable amount is less than zero!")]
        public string NetRefundableValidation { get; set; }

        [DataMember]
        [Display(Name = "Loans Guaranteed")]
        //[CustomValidation(typeof(WithdrawalNotificationDTO), "ValidateLoansGuaranteed", ErrorMessage = "The number of loans guaranteed must be zero!")]
        public int TotalLoansGuaranteed { get; set; }

        public static ValidationResult ValidateNetRefundable(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as WithdrawalNotificationDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be WithdrawalNotificationDTO");

            switch ((WithdrawalNotificationCategory)bindingModel.Category)
            {
                case WithdrawalNotificationCategory.Voluntary:
                case WithdrawalNotificationCategory.Retiree:
                    if (bindingModel.NetRefundable * -1 > 0m)
                        return new ValidationResult("The net refundable amount is less than zero!");
                    break;
                default:
                    break;
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateLoansGuaranteed(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as WithdrawalNotificationDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be WithdrawalNotificationDTO");

            switch ((WithdrawalNotificationCategory)bindingModel.Category)
            {
                case WithdrawalNotificationCategory.Voluntary:
                case WithdrawalNotificationCategory.Retiree:
                    if (bindingModel.TotalLoansGuaranteed != 0)
                        return new ValidationResult("The number of loans guaranteed must be zero!");
                    break;
                default:
                    break;
            }

            return ValidationResult.Success;
        }
    }
}
