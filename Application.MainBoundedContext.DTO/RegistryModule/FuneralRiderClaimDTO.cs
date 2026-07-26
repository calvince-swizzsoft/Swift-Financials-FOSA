using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class FuneralRiderClaimDTO : BindingModelBase<FuneralRiderClaimDTO>
    {
        public FuneralRiderClaimDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [DataMember]
        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [DataMember]
        [Display(Name = "Claim Type")]
        public int ClaimType { get; set; }

        [DataMember]
        [Display(Name = "Claim Type")]
        public string ClaimTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(FuneralRiderClaimType), ClaimType) ? EnumHelper.GetDescription((FuneralRiderClaimType)ClaimType) : string.Empty;
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
                return Enum.IsDefined(typeof(OriginationVerificationAuthorizationStatus), Status) ? EnumHelper.GetDescription((OriginationVerificationAuthorizationStatus)Status) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }

        [DataMember]
        [Display(Name = "Date Of Death")]
        public DateTime DateOfDeath { get; set; }

        [DataMember]
        [Display(Name = "Document")]
        [Required]
        public string FileName { get; set; }

        [DataMember]
        [Display(Name = "Title")]
        [Required]
        public string FileTitle { get; set; }

        [DataMember]
        [Display(Name = "Description")]
        [Required]
        public string FileDescription { get; set; }

        [DataMember]
        [Display(Name = "MIME Type")]
        public string FileMIMEType { get; set; }

        [DataMember]
        [Display(Name = "Buffer")]
        public byte[] FileBuffer { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        #region Customer

        [DataMember]
        [Display(Name = "Customer")]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public int CustomerType { get; set; }

        [DataMember]
        [Display(Name = "Customer Type")]
        public string CustomerTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(CustomerType), CustomerType) ? EnumHelper.GetDescription((CustomerType)CustomerType) : string.Empty;
            }
        }

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
        [Display(Name = "Customer First Name")]
        public string CustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string CustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Group Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        public string CustomerFullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)CustomerType)
                {
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", CustomerIndividualSalutationDescription, CustomerIndividualFirstName, CustomerIndividualLastName).Trim();
                        break;
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Partnership:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.Corporation:
                    case Infrastructure.Crosscutting.Framework.Utils.CustomerType.MicroCredit:
                        result = CustomerNonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
        [Display(Name = "Mobile Line")]
        public string CustomerAddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string CustomerIndividualIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Customer Employer")]
        public Guid CustomerStationZoneDivisionEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string CustomerStationZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Customer Payroll Numbers")]
        public string CustomerIndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string CustomerReference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string CustomerReference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string CustomerReference3 { get; set; }
        #endregion

        #region FuneralRiderClaimant

        [DataMember]
        [Display(Name = "Spouse/Claimant ID No.")]
        [Required]
        public string FuneralRiderClaimantIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Spouse/Claimant TSC Number")]
        public string FuneralRiderClaimantTscNumber { get; set; }

        [DataMember]
        [Display(Name = "Spouse/Claimant Name")]
        [Required]
        public string FuneralRiderClaimantName { get; set; }

        [DataMember]
        [Display(Name = "Spouse/Claimant Mobile Number")]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "The mobile number should start with a plus sign, followed by the country code and national number")]
        [Required]
        public string FuneralRiderClaimantMobileNumber { get; set; }

        [DataMember]
        [Display(Name = "Spouse/Claimant Relationship")]
        [Required]
        public string FuneralRiderClaimantRelationship { get; set; }

        [DataMember]
        [Display(Name = "Spouse/Claimant Signature Date")]
        [Required]
        public DateTime FuneralRiderClaimantSignatureDate { get; set; }

        #endregion

        #region ImmediateSuperior

        [DataMember]
        [Display(Name = "Immediate Superior ID No.")]
        [Required]
        public string ImmediateSuperiorIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Immediate Superior Name")]
        [Required]
        public string ImmediateSuperiorName { get; set; }

        [DataMember]
        [Display(Name = "Immediate Superior Signature Date")]
        [Required]
        public DateTime ImmediateSuperiorSignatureDate { get; set; }

        #endregion

        #region AreaChief

        [DataMember]
        [Display(Name = "Area Chief ID No.")]
        public string AreaChiefIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Area Chief Name")]
        [Required]
        public string AreaChiefName { get; set; }

        [DataMember]
        [Display(Name = "Area Chief Signature Date")]
        [Required]
        public DateTime AreaChiefSignatureDate { get; set; }

        #endregion

        #region AreaDelegate

        [DataMember]
        [Display(Name = "Area Delegate ID No.")]
        public string AreaDelegateIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = "Area Delegate Name")]
        [Required]
        public string AreaDelegateName { get; set; }

        [DataMember]
        [Display(Name = "Area Delegate TSC Number")]
        public string AreaDelegateTscNumber { get; set; }

        [DataMember]
        [Display(Name = "Area Delegate Signature Date")]
        [Required]
        public DateTime AreaDelegateSignatureDate { get; set; }

        #endregion

        #region AreaBoardMember

        [DataMember]
        [Display(Name = " Area Board Member ID No.")]
        public string AreaBoardMemberIdentityCardNumber { get; set; }

        [DataMember]
        [Display(Name = " Area Board Member Name")]
        [Required]
        public string AreaBoardMemberName { get; set; }

        [DataMember]
        [Display(Name = " Area Board Member TSC Number")]
        public string AreaBoardMemberTscNumber { get; set; }

        [DataMember]
        [Display(Name = " Area Board Member Signature Date")]
        [Required]
        public DateTime AreaBoardMemberSignatureDate { get; set; }

        #endregion
    }
}
