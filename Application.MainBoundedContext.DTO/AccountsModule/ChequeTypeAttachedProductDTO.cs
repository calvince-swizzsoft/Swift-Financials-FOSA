using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class ChequeTypeAttachedProductDTO : BindingModelBase<ChequeTypeAttachedProductDTO>
    {
        public ChequeTypeAttachedProductDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "ChequeType")]
        public Guid ChequeTypeId { get; set; }

        [DataMember]
        [Display(Name = "ChequeType")]
        public ChequeTypeDTO ChequeType { get; set; }

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
