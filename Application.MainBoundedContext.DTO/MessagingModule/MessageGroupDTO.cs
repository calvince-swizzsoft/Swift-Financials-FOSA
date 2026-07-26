
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.MessagingModule
{
    public class MessageGroupDTO : BindingModelBase<MessageGroupDTO>
    {
        public MessageGroupDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Display(Name = "Target")]
        public int Target { get; set; }

        [DataMember]
        [Display(Name = "Target")]
        public string TargetDescription
        {
            get
            {
                return Enum.IsDefined(typeof(MessageGroupTarget), Target) ? EnumHelper.GetDescription((MessageGroupTarget)Target) : string.Empty;
            }
        }

        [DataMember]
        [Display(Name = "Target Values")]
        [Required]
        public string TargetValues { get; set; }

        [DataMember]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }



        // Additional DTO
        public ObservableCollection<CustomerDTO> customer { get; set; }


        [DataMember]
        [Display(Name ="Customer")]
        public string Customer { get; set; }
        
        [DataMember]
        [Display(Name ="Customer")]
        public Guid CustomerID { get; set; } 
        
        [DataMember]
        [Display(Name ="Mobile Line")]
        public string CustomerMobileNumber { get; set; }
        
        [DataMember]
        [Display(Name ="E-mail Address")]
        public string CustomerEmailAddress { get; set; }

        public ObservableCollection<MessageGroupDTO> messageGroupCustomerDTO { get; set; }



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


        [Display(Name = "Customer Filter")]
        public int CustomerFilter { get; set; }

        [Display(Name = "Record Status")]
        public string CustomerFilterDescription
        {
            get
            {
                return EnumHelper.GetDescription((CustomerFilter)CustomerFilter);
            }
        }
    }
}
