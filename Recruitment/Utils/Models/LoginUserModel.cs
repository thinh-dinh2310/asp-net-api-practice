using BusinessObject;
using System;
using Utils;

namespace Utils.Models
{
    public class LoginUser
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public LoginUser(User user, string accessToken, string refreshToken)
        {
            Id = user.Id;
            DisplayName = user.DisplayName;
            Email = user.Email;
            RoleId = user.RoleId;
            RoleName = user.Role.RoleName;
            Address = user.Address;
            Phone = user.Phone;
            AccessToken = accessToken;
            IsDeleted = user.IsDeleted;
            RefreshToken = refreshToken;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }

        public LoginUser()
        {

        }
    }
}
