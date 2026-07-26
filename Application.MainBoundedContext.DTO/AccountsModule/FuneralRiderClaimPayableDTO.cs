using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class FuneralRiderClaimPayableDTO : BindingModelBase<FuneralRiderClaimPayableDTO>
    {
        public FuneralRiderClaimPayableDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Funeral Rider Claim")]
        [ValidGuid]
        public Guid FuneralRiderClaimId { get; set; }

        [DataMember]
        [Display(Name = "Claim Type")]
        public int FuneralRiderClaimClaimType { get; set; }

        [DataMember]
        [Display(Name = "Claim Type")]
        public string FuneralRiderClaimClaimTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FuneralRiderClaimType), FuneralRiderClaimClaimType) ? EnumHelper.GetDescription((FuneralRiderClaimType)FuneralRiderClaimClaimType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Branch")]
        [ValidGuid]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Reference #")]
        [Required]
        public int ReferenceNumber { get; set; }

        [DataMember]
        [Display(Name = "Reference #")]
        public string PaddedReferenceNumber
        {
            get
            {
                return string.Format("{0}", ReferenceNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Record Status")]
        public int RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(OriginationVerificationAuthorizationStatus), RecordStatus) ? EnumHelper.GetDescription((OriginationVerificationAuthorizationStatus)RecordStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Payment Status")]
        public int PaymentStatus { get; set; }

        [DataMember]
        [Display(Name = "Payment Status")]
        public string PaymentStatusDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FuneralRiderClaimPaymentStatus), PaymentStatus) ? EnumHelper.GetDescription((FuneralRiderClaimPaymentStatus)PaymentStatus) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected By")]
        public string AuditedBy { get; set; }

        [DataMember]
        [Display(Name = "Verification/Rejection Remarks")]
        public string AuditRemarks { get; set; }

        [DataMember]
        [Display(Name = "Verified/Rejected Date")]
        public DateTime? AuditedDate { get; set; }

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
        [Display(Name = "Deceased")]
        public string Deceased
        {
            get
            {
                if (FuneralRiderClaimClaimType == (int)FuneralRiderClaimType.MemberClaim)
                {
                    return CustomerFullName;
                }
                else
                {
                    return FuneralRiderClaimFuneralRiderClaimantName;
                }
            }
        }

        [DataMember]
        [Display(Name = "Beneficiary")]
        public string Beneficiary
        {
            get
            {
                if (FuneralRiderClaimClaimType == (int)FuneralRiderClaimType.MemberClaim)
                {
                    return FuneralRiderClaimFuneralRiderClaimantName;
                }
                else
                {
                    return CustomerFullName;
                }
            }
        }

        #region Customer

        [DataMember]
        [Display(Name = "Customer")]
        public Guid FuneralRiderClaimCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string FuneralRiderClaimCustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int FuneralRiderClaimCustomerType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string FuneralRiderClaimCustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string FuneralRiderClaimCustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), FuneralRiderClaimCustomerType) ? EnumHelper.GetDescription((CustomerType)FuneralRiderClaimCustomerType) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Salutation")]
        public int FuneralRiderClaimCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string FuneralRiderClaimCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), FuneralRiderClaimCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)FuneralRiderClaimCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string FuneralRiderClaimCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string FuneralRiderClaimCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string FuneralRiderClaimCustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Member")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)FuneralRiderClaimCustomerType)
                {
                    case CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", FuneralRiderClaimCustomerIndividualSalutationDescription, FuneralRiderClaimCustomerIndividualFirstName, FuneralRiderClaimCustomerIndividualLastName).Trim();
                        break;
                    case CustomerType.Partnership:
                    case CustomerType.Corporation:
                    case CustomerType.MicroCredit:
                        result = FuneralRiderClaimCustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
            set {; }
        }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string FuneralRiderClaimCustomerAddressMobileLine { get; set; }

        #endregion

        #region Claimant

        [DataMember]
        [Display(Name = "Claimant ID No.")]
        public string FuneralRiderClaimFuneralRiderClaimantIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Claimant TSC Number")]
        public string FuneralRiderClaimFuneralRiderClaimantTscNumber { get; set; }

        [DataMember]
        [Display(Name = "Claimant Name")]
        public string FuneralRiderClaimFuneralRiderClaimantName { get; set; }

        [DataMember]
        [Display(Name = "Claimant Mobile Number")]
        public string FuneralRiderClaimFuneralRiderClaimantMobileNumber { get; set; }

        [DataMember]
        [Display(Name = "Claimant Relationship")]
        public string FuneralRiderClaimFuneralRiderClaimantRelationship { get; set; }

        #endregion
    }
}