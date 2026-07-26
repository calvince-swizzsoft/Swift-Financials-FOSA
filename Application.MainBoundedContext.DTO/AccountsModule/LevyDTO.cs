
using Infrastructure.Crosscutting.Framework.Attributes;
using Infrastructure.Crosscutting.Framework.Extensions;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class LevyDTO : BindingModelBase<LevyDTO>
    {
        public LevyDTO()
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
        [Display(Name = "Charge Type")]
        public int ChargeType { get; set; }

        [DataMember]
        [Display(Name = "Charge Type")]
        public string ChargeTypeDescription
        {
            get
            {
                return Enum.IsDefined(typeof(ChargeType), ChargeType) ? EnumHelper.GetDescription((ChargeType)ChargeType) : string.Empty;
            }
        }


        [DataMember]
        [Display(Name = "Percentage")]
        public double ChargePercentage { get; set; }

        //private double _chargePercentage;

        //[DataMember]
        //[Display(Name = "Percentage")]
        //public double ChargePercentage
        //{
        //    get { return _chargePercentage; }
        //    set
        //    {
        //        // Ensure the value is between 0 and 100
        //        if (value < 0 || value > 100)
        //        {
        //            throw new ArgumentOutOfRangeException("Charge percentage must be between 0 and 100.");
        //        }

        //        // If the sum of all percentages would exceed 100, throw an exception
        //        if (TotalPercentage + value - _chargePercentage > 100)
        //        {
        //            throw new InvalidOperationException("Total percentage cannot exceed 100.");
        //        }

        //        _chargePercentage = value;
        //    }
        //}

        //// Assuming you have a collection of ChargePercentage values
        //public double TotalPercentage { get; set; } // Sum of all percentages

        //// Method to recalculate the total percentage whenever ChargePercentage is modified
        //private void RecalculateTotalPercentage()
        //{
        //    // Logic to sum up all ChargePercentage values
        //    // and assign the result to TotalPercentage
        //}



        [DataMember]
        [Display(Name = "Fixed Amount")]
        public decimal ChargeFixedAmount { get; set; }

        [DataMember]
        [Display(Name = "Value")]
        public double ChargeValue { get; set; }

        [DataMember]
        [Display(Name = "Is Locked?")]
        public bool IsLocked { get; set; }

        [DataMember]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [Display(Name = "Levy Splits Total Percentage")]
        [CustomValidation(typeof(LevyDTO), "CheckLevySplitsTotalPercentage", ErrorMessage = "The sum of levy split percentage entries must be equal to 100%!")]
        public double LevySplitsTotalPercentage { get; set; }

        public static ValidationResult CheckLevySplitsTotalPercentage(object value, ValidationContext context)
        {
            var bindingModel = context.ObjectInstance as LevyDTO;
            if (bindingModel == null)
                throw new NotSupportedException("ObjectInstance must be LevyDTO");

            if ((double)value != 100d)
                return new ValidationResult(string.Format("The sum of commission split percentage entries ({0}) must be equal to 100%!", value));

            return ValidationResult.Success;
        } 
        public string ErrorMessageResult { get; set; }

        public List<LevySplitDTO> LevySplits { get; set; }

        public LevySplitDTO LevySplit { get; set; }
    }
}
