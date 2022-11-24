using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Validation
{
    public sealed class FutureDateTimeAttribute : ValidationAttribute
    {
        private const string _errorMessage = "'{0}' must be greater than now";

        public FutureDateTimeAttribute()
            : base(_errorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(_errorMessage, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && (DateTime)value <= DateTime.Now)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }
    }
}
