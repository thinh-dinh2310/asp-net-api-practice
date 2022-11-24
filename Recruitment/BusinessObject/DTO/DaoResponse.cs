using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class DaoResponse<T> where T : class
    {
        public Boolean IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T? Value { get; set; }

        public DaoResponse (Boolean isSuccess, string errorMessage, T? value) 
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Value = value;
        }

        public DaoResponse(Boolean isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Value = null;
        }

        public DaoResponse(Boolean isSuccess)
        {
            IsSuccess = isSuccess;
            ErrorMessage = null;
            Value = null;
        }
    }
}
