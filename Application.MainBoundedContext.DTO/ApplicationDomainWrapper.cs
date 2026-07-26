using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class ApplicationDomainWrapper : BindingModelBase<ApplicationDomainWrapper>
    {
        public ApplicationDomainWrapper()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }

        [DataMember]
        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
