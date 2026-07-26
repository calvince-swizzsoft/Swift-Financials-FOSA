using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.MessagingModule;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class CustomerDTO : IComparable<CustomerDTO>
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Branch")]
        public Guid BranchId { get; set; }

        [Display(Name = "Branch")]
        public string BranchDescription { get; set; }

        [Display(Name = "Branch E-mail")]
        public string BranchAddressEmail { get; set; }

        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [Display(Name = "Type")]
        public byte Type { get; set; }

        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerType)Type);
            }
        }

        [Display(Name = "Serial Number")]
        public int SerialNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string PaddedSerialNumber
        {
            get
            {
                return string.Format("{0}", SerialNumber).PadLeft(7, '0');
            }
        }

        [Display(Name = "Personal Identification Number")]
        public string PersonalIdentificationNumber { get; set; }

        [Display(Name = "Individual Type")]
        public byte IndividualType { get; set; }

        [Display(Name = "Individual Type")]
        public string IndividualTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IndividualType)IndividualType);
            }
        }

        [Display(Name = "First Name")]
        public string IndividualFirstName { get; set; }

        [Display(Name = "Other Names")]
        public string IndividualLastName { get; set; }

        [Display(Name = "Identity Card Type")]
        public byte IndividualIdentityCardType { get; set; }

        [Display(Name = "Identity Card Type")]
        public string IndividualIdentityCardTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)IndividualIdentityCardType);
            }
        }

        [Display(Name = "Identity Card Number")]
        public string IndividualIdentityCardNumber { get; set; }

        [Display(Name = "Identity Card Serial #")]
        public string IndividualIdentityCardSerialNumber { get; set; }

        [Display(Name = "Payroll Numbers")]
        public string IndividualPayrollNumbers { get; set; }

        [Display(Name = "Salutation")]
        public byte IndividualSalutation { get; set; }

        [Display(Name = "Salutation")]
        public string IndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)IndividualSalutation);
            }
        }

        [Display(Name = "Gender")]
        public byte IndividualGender { get; set; }

        [Display(Name = "Gender")]
        public string IndividualGenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)IndividualGender);
            }
        }

        [Display(Name = "Marital Status")]
        public byte IndividualMaritalStatus { get; set; }

        [Display(Name = "Marital Status")]
        public string IndividualMaritalStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((MaritalStatus)IndividualMaritalStatus);
            }
        }

        [Display(Name = "Nationality")]
        public byte IndividualNationality { get; set; }

        [Display(Name = "Nationality")]
        public string IndividualNationalityDescription
        {
            get
            {
                return EnumHelper.GetDescription((Nationality)IndividualNationality);
            }
        }

        [Display(Name = "Birth Date")]
        public DateTime? IndividualBirthDate { get; set; }

        [Display(Name = "Start Date")]
        public DateTime DurationStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime DurationEndDate { get; set; }

        [Display(Name = "Employment Designation")]
        public string IndividualEmploymentDesignation { get; set; }

        [Display(Name = "Employment Terms-Of-Service")]
        public byte? IndividualEmploymentTermsOfService { get; set; }

        [Display(Name = "Employment Terms-Of-Service")]
        public string IndividualEmploymentTermsOfServiceDescription
        {
            get
            {
                return IndividualEmploymentTermsOfService.HasValue ? EnumHelper.GetDescription((TermsOfService)IndividualEmploymentTermsOfService.Value) : string.Empty;
            }
        }

        [Display(Name = "Employment Date")]
        public DateTime? IndividualEmploymentDate { get; set; }

        [Display(Name = "Classification")]
        public byte IndividualClassification { get; set; }

        [Display(Name = "Classification")]
        public string IndividualClassificationDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerClassification)IndividualClassification);
            }
        }

        [Display(Name = "Group Name")]
        public string NonIndividualDescription { get; set; }

        [Display(Name = "Registration Number")]
        public string NonIndividualRegistrationNumber { get; set; }

        [Display(Name = "Registration Serial #")]
        public string NonIndividualRegistrationSerialNumber { get; set; }

        [Display(Name = "Date Established")]
        public DateTime? NonIndividualDateEstablished { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressAddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        public void ValidateAll()
        {
            throw new NotImplementedException();
        }

        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [Display(Name = "Postal Code")]
        public string AddressPostalCode { get; set; }

        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [Display(Name = "E-mail")]
        public string AddressEmail { get; set; }

        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [Display(Name = "Mobile Line")]
        public string AddressMobileLine { get; set; }

        [Display(Name = "Passport")]
        public byte[] PassportBuffer { get; set; }

        [Display(Name = "Signature")]
        public byte[] SignatureBuffer { get; set; }

        [Display(Name = "Identity Card (Front)")]
        public byte[] IdentityCardFrontSideBuffer { get; set; }

        [Display(Name = "Identity Card (Back)")]
        public byte[] IdentityCardBackSideBuffer { get; set; }

        [Display(Name = "Biometric Fingerprint Image")]
        public byte[] BiometricFingerprintBuffer { get; set; }

        [Display(Name = "Biometric Fingerprint Template")]
        public byte[] BiometricFingerprintTemplateBuffer { get; set; }

        [Display(Name = "Biometric Finger-Vein Template")]
        public byte[] BiometricFingerVeinTemplateBuffer { get; set; }

        public bool BiometricEnrollment { get; set; }

        [Display(Name = "Station")]
        public Guid? StationId { get; set; }

        [Display(Name = "Station")]
        public string StationDescription { get; set; }

        [Display(Name = "Zone")]
        public Guid? StationZoneId { get; set; }

        [Display(Name = "Zone")]
        public string StationZoneDescription { get; set; }

        [Display(Name = "Division")]
        public Guid? StationZoneDivisionId { get; set; }

        [Display(Name = "Division")]
        public string StationZoneDivisionDescription { get; set; }

        [Display(Name = "Employer")]
        public Guid? StationZoneDivisionEmployerId { get; set; }

        [Display(Name = "Employer")]
        public string StationZoneDivisionEmployerDescription { get; set; }

        [Display(Name = "Employer Retirement Age")]
        public byte? StationZoneDivisionEmployerRetirementAge { get; set; }

        [Display(Name = "Enforce retirement age?")]
        public bool? StationZoneDivisionEmployerEnforceRetirementAge { get; set; }

        [Display(Name = "Account Number")]
        public string Reference1 { get; set; }

        [Display(Name = "Membership Number")]
        public string Reference2 { get; set; }

        [Display(Name = "Personal File Number")]
        public string Reference3 { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Is Defaulter?")]
        public bool IsDefaulter { get; set; }

        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [Display(Name = "Inhibit Guaranteeing?")]
        public bool InhibitGuaranteeing { get; set; }

        [Display(Name = "Passport Image")]
        public Guid? PassportImageId { get; set; }

        [Display(Name = "Signature Image")]
        public Guid? SignatureImageId { get; set; }

        [Display(Name = "Identity Card (Front) Image")]
        public Guid? IdentityCardFrontSideImageId { get; set; }

        [Display(Name = "Identity Card (Back) Image")]
        public Guid? IdentityCardBackSideImageId { get; set; }

        [Display(Name = "Biometric Fingerprint Image")]
        public Guid? BiometricFingerprintImageId { get; set; }

        [Display(Name = "Biometric Fingerprint Template")]
        public Guid? BiometricFingerprintTemplateId { get; set; }

        [Display(Name = "Biometric Fingerprint Template Format")]
        public byte BiometricFingerprintTemplateFormat { get; set; }

        [Display(Name = "Biometric Finger-Vein Template")]
        public Guid? BiometricFingerVeinTemplateId { get; set; }

        [Display(Name = "Biometric Finger-Vein Template Format")]
        public byte BiometricFingerVeinTemplateFormat { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }


        [Display(Name = "Recruited By")]
        public string RecruitedBy { get; set; }

        [Display(Name = "Record Status")]
        public byte RecordStatus { get; set; }

        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((RecordStatus)RecordStatus);
            }
        }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Administrative Division")]
        public Guid? AdministrativeDivisionId { get; set; }

        [Display(Name = "Administrative Division")]
        public string AdministrativeDivisionDescription { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)Type)
                {
                    case CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", IndividualSalutationDescription, IndividualFirstName, IndividualLastName).Trim();
                        break;
                    case CustomerType.Partnership:
                        result = string.Format("{0}", NonIndividualDescription).Trim();
                        break;
                    case CustomerType.Corporation:
                        result = string.Format("{0}", NonIndividualDescription).Trim();
                        break;
                    case CustomerType.MicroCredit:
                        result = string.Format("{0}", NonIndividualDescription).Trim();
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [Display(Name = "Identification Number")]
        public string IdentificationNumber
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)Type)
                {
                    case CustomerType.Individual:
                        result = IndividualIdentityCardNumber;
                        break;
                    case CustomerType.Partnership:
                    case CustomerType.Corporation:
                    case CustomerType.MicroCredit:
                        result = NonIndividualRegistrationNumber;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [Display(Name = "Age")]
        public int Age
        {
            get
            {
                var result = -1;

                switch ((CustomerType)Type)
                {
                    case CustomerType.Individual:
                        if (IndividualBirthDate != null && IndividualBirthDate.Value <= DefaultSettings.Instance.ServerDate)
                            result = UberUtil.GetAge(IndividualBirthDate.Value);
                        break;
                    case CustomerType.Partnership:
                    case CustomerType.Corporation:
                    case CustomerType.MicroCredit:
                        if (NonIndividualDateEstablished != null && NonIndividualDateEstablished.Value <= DefaultSettings.Instance.ServerDate)
                            result = UberUtil.GetAge(NonIndividualDateEstablished.Value);
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [Display(Name = "Membership Period (Months)")]
        public int MembershipPeriod
        {
            get
            {
                if (RegistrationDate != null && RegistrationDate.Value <= DefaultSettings.Instance.ServerDate)
                    return UberUtil.GetPeriod(DefaultSettings.Instance.ServerDate, RegistrationDate.Value);
                else return -1;
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Format("{0}", this.FullName));
            stringBuilder.AppendLine(string.Format("{0}", this.PaddedSerialNumber));
            stringBuilder.AppendLine(string.Format("{0}", this.Reference1));

            return string.Format("{0}", stringBuilder);
        }

        public string ErrorMessageResult;
        public string ErrorMessages;
        public int CompareTo(CustomerDTO other)
        {
            return this.CompareTo(other);
        }




        [Display(Name = "Guarantor")]
        public string GuarantorDesc { get; set; }
        //// Additional Document DTO
        //public Guid DocumentID { get; set; }  // Unique identifier for the document
        //public Guid CustomerId { get; set; }  // Reference to the associated customer
        //public byte[] PassportPhoto { get; set; }
        //public byte[] SignaturePhoto { get; set; }
        //public byte[] IDCardFrontPhoto { get; set; }
        //public byte[] IDCardBackPhoto { get; set; }

        //public DateTime DocumentCreatedDate { get; set; } = DateTime.Now;

        // Additional
        public MessageGroupDTO messageGroup { get; set; }
        public object ZoneDivisionEmployerDescription { get; set; }
        public bool HasErrors { get; set; }

        [JsonIgnore] // Break circular reference
        public CommissionDTO CommissionDTO { get; set; }
        //[DataMember]
        //public CommissionDTO CommissionDTO { get; set; }

        public List<NextOfKinDTO> NextOfKins { get; set; }
        public string PhoneNumber { get; set; }

        [DataMember]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [DataMember]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

    }

}
