using Application.Seedwork;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class FixedDepositTypeLevyDTO : BindingModelBase<FixedDepositTypeLevyDTO>
    {
        public FixedDepositTypeLevyDTO()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "FixedDepositType")]
        public Guid FixedDepositTypeId { get; set; }

        [DataMember]
        [Display(Name = "FixedDepositType")]
        public FixedDepositTypeDTO FixedDepositType { get; set; }

        [DataMember]
        [Display(Name = "Levy")]
        public Guid LevyId { get; set; }

        [DataMember]
        [Display(Name = "Levy")]
        public LevyDTO Levy { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
    }
}
