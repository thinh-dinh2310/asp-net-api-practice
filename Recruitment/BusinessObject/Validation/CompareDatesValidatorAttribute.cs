using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Validation
{
    public sealed class CompareDatesValidatorAttribute : ValidationAttribute
    {
        private string _dateToCompare;
        private const string _errorMessage = "'{0}' must be greater than '{1}'";

        public CompareDatesValidatorAttribute(string dateToCompare)
            : base(_errorMessage)
        {
            _dateToCompare = dateToCompare;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(_errorMessage, name, _dateToCompare);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateToCompare = validationContext.ObjectType.GetProperty(_dateToCompare);
            var dateToCompareValue = dateToCompare.GetValue(validationContext.ObjectInstance, null);
            if (dateToCompareValue != null && value != null && (DateTime)value <= (DateTime)dateToCompareValue)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }
    }
}
