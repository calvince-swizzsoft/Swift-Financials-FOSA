
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

namespace Application.MainBoundedContext.DTO.RegistryModule
{
    public class FileMovementHistoryDTO : BindingModelBase<FileMovementHistoryDTO>
    {
        public FileMovementHistoryDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "File Register")]
        public Guid FileRegisterId { get; set; }

        [DataMember]
        [Display(Name = "Customer")]
        [ValidGuid]
        public Guid FileRegisterCustomerId { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public int FileRegisterCustomerIndividualSalutation { get; set; }

        [DataMember]
        [Display(Name = "Customer Salutation")]
        public string FileRegisterCustomerIndividualSalutationDescription
        {
            get
            {
                return Enum.IsDefined(typeof(Salutation), FileRegisterCustomerIndividualSalutation) ? EnumHelper.GetDescription((Salutation)FileRegisterCustomerIndividualSalutation) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Customer First Name")]
        public string FileRegisterCustomerIndividualFirstName { get; set; }

        [DataMember]
        [Display(Name = "Customer Other Names")]
        public string FileRegisterCustomerIndividualLastName { get; set; }

        [DataMember]
        [Display(Name = "Customer Name")]
        public string FileRegisterCustomerFullName
        {
            get
            {
                return string.Format("{0} {1} {2}", FileRegisterCustomerIndividualSalutationDescription, FileRegisterCustomerIndividualFirstName, FileRegisterCustomerIndividualLastName);
            }
        }

        [DataMember]
        [Display(Name = "Source Department")]
        [ValidGuid]
        public Guid SourceDepartmentId { get; set; }

        [DataMember]
        [Display(Name = "Source")]
        public string SourceDepartmentDescription { get; set; }

        [DataMember]
        [Display(Name = "Destination Department")]
        [ValidGuid]
        public Guid DestinationDepartmentId { get; set; }

        [DataMember]
        [Display(Name = "Destination")]
        public string DestinationDepartmentDescription { get; set; }

        [DataMember]
        [Display(Name = "Remarks")]
        [Required]
        public string Remarks { get; set; }

        [DataMember]
        [Display(Name = "Carrier")]
        [Required]
        public string Carrier { get; set; }

        [DataMember]
        [Display(Name = "Sender")]
        public string Sender { get; set; }

        [Display(Name = "Dispatch Date")]
        public DateTime? SendDate { get; set; }

        [DataMember]
        [Display(Name = "Recipient")]
        public string Recipient { get; set; }

        [DataMember]
        [Display(Name = "Receive Date")]
        public DateTime? ReceiveDate { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
