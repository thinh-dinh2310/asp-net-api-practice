using BusinessObject;
using System;

namespace Utils.Models
{
    public class LoginUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginUserRequest()
        {

        }
    }
}
