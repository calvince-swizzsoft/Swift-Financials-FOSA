using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AdministrationModule
{
    public class CompanyAttachedProductDTO : BindingModelBase<CompanyAttachedProductDTO>
    {
        public CompanyAttachedProductDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        [DataMember]
        [Display(Name = "Company")]
        public CompanyDTO Company { get; set; }

        [DataMember]
        [Display(Name = "ProductCode")]
        public int ProductCode { get; set; }

        [DataMember]
        [Display(Name = "TargetProductId")]
        public Guid TargetProductId { get; set; }

        [DataMember]
        [Display(Name = "CreatedDate")]
        public DateTime CreatedDate { get; set; }
    }
}
