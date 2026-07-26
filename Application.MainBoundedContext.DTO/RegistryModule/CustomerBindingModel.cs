using Application.MainBoundedContext.DTO.MicroCreditModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class CustomerBindingModel : BindingModelBase<CustomerBindingModel>
    {
        public CustomerBindingModel()
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
        [Display(Name = "Branch E-mail")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address!")]
        public string BranchAddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public string BranchCompanyDescription { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        [CustomValidation(typeof(CustomerBindingModel), "CheckIndividualAge", ErrorMessage = "The minimum required membership age is 18 years!")]
        public byte Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerType)Type);
            }
        }

        [DataMember]
        [Display(Name = "Serial Number")]
        public int SerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Serial Number")]
        public string PaddedSerialNumber
        {
            get
            {
                return string.Format("{0}", SerialNumber).PadLeft(7, '0');
            }
        }

        [DataMember]
        [Display(Name = "Personal Identification Number")]
        public string PersonalIdentificationNumber { get; set; }

        [DataMember]
        [Display(Name = "Individual Type")]
        public byte IndividualType { get; set; }

        [DataMember]
        [Display(Name = "Individual Type")]
        public string IndividualTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IndividualType)IndividualType);
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string IndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string IndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public byte IndividualIdentityCardType { get; set; }

        [DataMember]
        [Display(Name = "Identity Card Type")]
        public string IndividualIdentityCardTypeDescription
        {
            get
            {
                return EnumHelper.GetDescription((IdentityCardType)IndividualIdentityCardType);
            }
        }

        [DataMember]
        [Display(Name = "Identity Card Number")]
        public string IndividualIdentityCardNumber { get; set; }

        [Display(Name = "Identity Card Serial #")]
        public string IndividualIdentityCardSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Payroll Numbers")]
        public string IndividualPayrollNumbers { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public byte IndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string IndividualSalutationDescription
        {
            get
            {
                return EnumHelper.GetDescription((Salutation)IndividualSalutation);
            }
        }

        [DataMember]
        [Display(Name = "Gender")]
        public byte IndividualGender { get; set; }

        [DataMember]
        [Display(Name = "Gender")]
        public string IndividualGenderDescription
        {
            get
            {
                return EnumHelper.GetDescription((Gender)IndividualGender);
            }
        }

        [DataMember]
        [Display(Name = "Marital Status")]
        public byte IndividualMaritalStatus { get; set; }

        [DataMember]
        [Display(Name = "Marital Status")]
        public string IndividualMaritalStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((MaritalStatus)IndividualMaritalStatus);
            }
        }

        [DataMember]
        [Display(Name = "Nationality")]
        public byte IndividualNationality { get; set; }

        [DataMember]
        [Display(Name = "Nationality")]
        public string IndividualNationalityDescription
        {
            get
            {
                return EnumHelper.GetDescription((Nationality)IndividualNationality);
            }
        }

        [DataMember]
        [Display(Name = "Birth Date")]
        public DateTime? IndividualBirthDate { get; set; }

        [DataMember]
        [Display(Name = "Employment Designation")]
        public string IndividualEmploymentDesignation { get; set; }

        [DataMember]
        [Display(Name = "Employment Terms-Of-Service")]
        public byte? IndividualEmploymentTermsOfService { get; set; }

        [DataMember]
        [Display(Name = "Employment Terms-Of-Service")]
        public string IndividualEmploymentTermsOfServiceDescription
        {
            get
            {
                return IndividualEmploymentTermsOfService.HasValue ? EnumHelper.GetDescription((TermsOfService)IndividualEmploymentTermsOfService.Value) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Employment Date")]
        public DateTime? IndividualEmploymentDate { get; set; }

        [DataMember]
        [Display(Name = "Classification")]
        public byte IndividualClassification { get; set; }

        [DataMember]
        [Display(Name = "Classification")]
        public string IndividualClassificationDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerClassification)IndividualClassification);
            }
        }

        [DataMember]
        [Display(Name = "Group Name")]
        public string NonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string NonIndividualRegistrationNumber { get; set; }

        [Display(Name = "Registration Serial #")]
        public string NonIndividualRegistrationSerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? NonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Address Line 1")]
        public string AddressAddressLine1 { get; set; }

        [DataMember]
        [Display(Name = "Address Line 2")]
        public string AddressAddressLine2 { get; set; }

        [DataMember]
        [Display(Name = "Street")]
        public string AddressStreet { get; set; }

        [DataMember]
        [Display(Name = "Postal Code")]
        public string AddressPostalCode { get; set; }

        [DataMember]
        [Display(Name = "City")]
        public string AddressCity { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid email address!")]
        public string AddressEmail { get; set; }

        [DataMember]
        [Display(Name = "Land Line")]
        public string AddressLandLine { get; set; }

        [DataMember]
        [Display(Name = "Mobile Line")]
        [RegularExpression(@"^\+(?:[0-9]??){6,14}[0-9]$", ErrorMessage = "The mobile number should start with a plus sign, followed by the country code and national number")]
        public string AddressMobileLine { get; set; }

        [DataMember]
        [Display(Name = "Passport")]
        public byte[] PassportBuffer { get; set; }

        [DataMember]
        [Display(Name = "Signature")]
        public byte[] SignatureBuffer { get; set; }

        [DataMember]
        [Display(Name = "Identity Card (Front)")]
        public byte[] IdentityCardFrontSideBuffer { get; set; }

        [DataMember]
        [Display(Name = "Identity Card (Back)")]
        public byte[] IdentityCardBackSideBuffer { get; set; }

        [DataMember]
        [Display(Name = "Biometric Fingerprint Image")]
        public byte[] BiometricFingerprintBuffer { get; set; }

        [DataMember]
        [Display(Name = "Biometric Fingerprint Template")]
        public byte[] BiometricFingerprintTemplateBuffer { get; set; }

        [DataMember]
        [Display(Name = "Biometric Finger-Vein Template")]
        public byte[] BiometricFingerVeinTemplateBuffer { get; set; }

        [DataMember]
        public bool BiometricEnrollment { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        [ValidGuid]
        public Guid? StationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string StationDescription { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        public Guid? StationZoneId { get; set; }

        [DataMember]
        [Display(Name = "Zone")]
        public string StationZoneDescription { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public Guid? StationZoneDivisionId { get; set; }

        [DataMember]
        [Display(Name = "Division")]
        public string StationZoneDivisionDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public Guid? StationZoneDivisionEmployerId { get; set; }

        [DataMember]
        [Display(Name = "Employer")]
        public string StationZoneDivisionEmployerDescription { get; set; }

        [DataMember]
        [Display(Name = "Employer Retirement Age")]
        public int? StationZoneDivisionEmployerRetirementAge { get; set; }

        [DataMember]
        [Display(Name = "Enforce retirement age?")]
        public bool? StationZoneDivisionEmployerEnforceRetirementAge { get; set; }

        [DataMember]
        [Display(Name = "Account Number")]
        public string Reference1 { get; set; }

        [DataMember]
        [Display(Name = "Membership Number")]
        public string Reference2 { get; set; }

        [DataMember]
        [Display(Name = "Personal File Number")]
        public string Reference3 { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Is Defaulter?")]
        public bool IsDefaulter { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Inhibit Guaranteeing?")]
        public bool InhibitGuaranteeing { get; set; }

        [DataMember]
        [Display(Name = "Passport Image")]
        public Guid? PassportImageId { get; set; }

        [DataMember]
        [Display(Name = "Signature Image")]
        public Guid? SignatureImageId { get; set; }

        [DataMember]
        [Display(Name = "Identity Card (Front) Image")]
        public Guid? IdentityCardFrontSideImageId { get; set; }

        [DataMember]
        [Display(Name = "Identity Card (Back) Image")]
        public Guid? IdentityCardBackSideImageId { get; set; }

        [DataMember]
        [Display(Name = "Biometric Fingerprint Image")]
        public Guid? BiometricFingerprintImageId { get; set; }

        [DataMember]
        [Display(Name = "Biometric Fingerprint Template")]
        public Guid? BiometricFingerprintTemplateId { get; set; }

        [DataMember]
        [Display(Name = "Biometric Fingerprint Template Format")]
        public int BiometricFingerprintTemplateFormat { get; set; }

        [DataMember]
        [Display(Name = "Biometric Finger-Vein Template")]
        public Guid? BiometricFingerVeinTemplateId { get; set; }

        [DataMember]
        [Display(Name = "Biometric Finger-Vein Template Format")]
        public int BiometricFingerVeinTemplateFormat { get; set; }

        [DataMember]
        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }


        [DataMember]
        [Display(Name = "Recruited By")]
        public string RecruitedBy { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public byte RecordStatus { get; set; }

        [DataMember]
        [Display(Name = "Record Status")]
        public string RecordStatusDescription
        {
            get
            {
                return EnumHelper.GetDescription((RecordStatus)RecordStatus);
            }
        }

        [DataMember]
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [DataMember]
        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Administrative Division")]
        public Guid? AdministrativeDivisionId { get; set; }

        [DataMember]
        [Display(Name = "Administrative Division")]
        public string AdministrativeDivisionDescription { get; set; }
        public string ErrorMessageResult { get; set; }
        [DataMember]
        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                var result = string.Empty;

                switch ((CustomerType)Type)
                {
                    case CustomerType.Individual:
                        result = string.Format("{0} {1} {2}", IndividualSalutationDescription, IndividualFirstName, IndividualLastName);
                        break;
                    case CustomerType.Partnership:
                    case CustomerType.Corporation:
                    case CustomerType.MicroCredit:
                        result = NonIndividualDescription;
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        public List<NextOfKinDTO>nextOfKins;

        //HashSet<NextOfKinDTO> _nextOfKin;
        //[DataMember]
        //[Display(Name = "Next-of-Kin Collection")]
        //[CustomValidation(typeof(CustomerBindingModel), "CheckNominatedPercentage", ErrorMessage = "Total nominated percentage must not exceed 100%!")]
        //public virtual ICollection<NextOfKinDTO> NextOfKin
        //{
        //    get
        //    {
        //        if (_nextOfKin == null)
        //        {
        //            _nextOfKin = new HashSet<NextOfKinDTO>();
        //        }
        //        return _nextOfKin;
        //    }
        //    private set
        //    {
        //        _nextOfKin = new HashSet<NextOfKinDTO>(value);
        //    }
        //}

        //public static ValidationResult CheckNominatedPercentage(object value, ValidationContext context)
        //{
        //    var bindingModel = context.ObjectInstance as CustomerBindingModel;
        //    if (bindingModel == null)
        //        throw new NotSupportedException("ObjectInstance must be CustomerBindingModel");

        //    if (bindingModel.NextOfKin.Sum(x => x.NominatedPercentage) > 100d)
        //        return new ValidationResult("Total nominated percentage must not exceed 100%!");

        //    return ValidationResult.Success;
        //}

        public static ValidationResult CheckIndividualAge(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as CustomerBindingModel;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be CustomerBindingModel");

            switch ((CustomerType)bindingModel.Type)
            {
                case CustomerType.Individual:

                    switch ((IndividualType)bindingModel.IndividualType)
                    {
                        case Infrastructure.Crosscutting.Framework.Utils.IndividualType.Adult:
                            if (bindingModel.Age < DefaultSettings.Instance.MinRequiredMembershipAge)
                                return new ValidationResult(string.Format("Minimum required membership age is {0}!", DefaultSettings.Instance.MinRequiredMembershipAge));
                            break;
                        case Infrastructure.Crosscutting.Framework.Utils.IndividualType.Minor:
                            break;
                        default:
                            break;
                    }

                    break;
                case CustomerType.Partnership:
                    break;
                case CustomerType.Corporation:
                    break;
                case CustomerType.MicroCredit:
                    break;
                default:
                    break;
            }

            return ValidationResult.Success;
        }


        //   // Additional Document DTO
        //public Guid DocumentID { get; set; }  // Unique identifier for the document
        //public Guid CustomerId { get; set; }  // Reference to the associated customer
        //public byte[] PassportPhoto { get; set; }
        //public byte[] SignaturePhoto { get; set; }
        //public byte[] IDCardFrontPhoto { get; set; }
        //public byte[] IDCardBackPhoto { get; set; }

        //public DateTime DocumentCreatedDate { get; set; } = DateTime.Now;




        [Display(Name = "Referee")]
        public string RefereeFirstName { get; set; }

        [Display(Name = "Id")]
        public Guid RefereeId { get; set; }
        public ObservableCollection<PartnershipMemberDTO> partnershipMemberCollection { get; set; }
        public ObservableCollection<CorporationMemberDTO> corporationMemberDTO { get; set; }
        public ObservableCollection<MicroCreditGroupMemberDTO> microCreditGroupMemberDTOs { get; set; }
        public ObservableCollection<RefereeDTO> refereeDTOs { get; set; }


    }
}
