
using Infrastructure.Crosscutting.Framework.Attributes;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Application.MainBoundedContext.DTO.RegistryModule;


namespace Application.MainBoundedContext.DTO.MicroCreditModule
{
    public class MicroCreditGroupDTO : BindingModelBase<MicroCreditGroupDTO>
    {
        public MicroCreditGroupDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Parent")]
        public Guid? ParentId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid CustomerId { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        public string CustomerNonIndividualDescription { get; set; }

        [DataMember]
        [Display(Name = "Registration Number")]
        public string CustomerNonIndividualRegistrationNumber { get; set; }

        [DataMember]
        [Display(Name = "Date Established")]
        public DateTime? CustomerNonIndividualDateEstablished { get; set; }

        [DataMember]
        [Display(Name = "Registration Date")]
        public DateTime? CustomerRegistrationDate { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public Guid? CustomerStationId { get; set; }

        [DataMember]
        [Display(Name = "Station")]
        public string CustomerStationDescription { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool CustomerIsLocked { get; set; }

        [DataMember]
        [Display(Name = "Credit Officer")]
        [ValidGuid]
        public Guid MicroCreditOfficerId { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public int MicroCreditOfficerEmployeeCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Salutation")]
        public string MicroCreditOfficerEmployeeCustomerSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), MicroCreditOfficerEmployeeCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)MicroCreditOfficerEmployeeCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "First Name")]
        public string MicroCreditOfficerEmployeeCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Other Names")]
        public string MicroCreditOfficerEmployeeCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Credit Officer")]
        public string MicroCreditOfficerEmployeeCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", MicroCreditOfficerEmployeeCustomerSalutationDescription, MicroCreditOfficerEmployeeCustomerIndividualFirstName, MicroCreditOfficerEmployeeCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Type")]
        public int Type { get; set; }

        [DataMember]
        [Display(Name = "Type")]
        public string TypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MicroCreditGroupType), Type) ? EnumHelper.GetDescription((MicroCreditGroupType)Type) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Purpose")]
        [Required]
        public string Purpose { get; set; }

        [DataMember]
        [Display(Name = "Activities")]
        [Required]
        public string Activities { get; set; }

        [DataMember]
        [Display(Name = "Meeting Frequency")]
        public int MeetingFrequency { get; set; }

        [DataMember]
        [Display(Name = "Meeting Frequency")]
        public string MeetingFrequencyDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MicroCreditGroupMeetingFrequency), MeetingFrequency) ? EnumHelper.GetDescription((MicroCreditGroupMeetingFrequency)MeetingFrequency) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Meeting Day Of Week")]
        public int MeetingDayOfWeek { get; set; }

        [DataMember]
        [Display(Name = "Meeting Day Of Week")]
        public string MeetingDayOfWeekDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MicroCreditGroupMeetingDayOfWeek), MeetingDayOfWeek) ? EnumHelper.GetDescription((MicroCreditGroupMeetingDayOfWeek)MeetingDayOfWeek) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Meeting Place")]
        [Required]
        public string MeetingPlace { get; set; }

        [DataMember]
        [Display(Name = "Minimum Members")]
        public int MinimumMembers { get; set; }

        [DataMember]
        [Display(Name = "Maximum Members")]
        public int MaximumMembers { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        //Added Properties
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Employer")]
        public string Employer { get; set; }

        [Display(Name = "Account Number")]
        public string CustomerAccountCustomerReference1 { get; set; }

        [Display(Name = "MemberShip Number")]
        public string CustomerAccountCustomerReference2 { get; set; }

        [Display(Name = "Personal File Number")]
        public string CustomerAccountCustomerReference3 { get; set; }

        [Display(Name = "Loan Cycle")]
        public int LoanCycle { get; set; }

        [Display(Name = "Customer FullName")]
        public string CustomerFullName { get; set; }

        public CustomerDTO customer { get; set; }

        [Display(Name = "Parent Group")]
        public string ParentGroup { get; set; }

        public List<MicroCreditGroupMemberDTO> GroupMember { get; set; }
    }
}
