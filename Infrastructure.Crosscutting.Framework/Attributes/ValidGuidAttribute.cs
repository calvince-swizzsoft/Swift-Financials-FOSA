using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Infrastructure.Crosscutting.Framework.Attributes
{
    public class ValidGuidAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The {0} identifier is invalid!";

        public ValidGuidAttribute()
            : base(DefaultErrorMessage)
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = Convert.ToString(value, CultureInfo.CurrentCulture);

            Guid guid;
            if (string.IsNullOrWhiteSpace(input) || !Guid.TryParse(input, out guid))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            else if (guid == Guid.Empty)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}
