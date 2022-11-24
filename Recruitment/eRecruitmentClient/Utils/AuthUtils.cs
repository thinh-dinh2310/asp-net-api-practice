using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Models;

namespace eRecruitmentClient.Utils
{
    public static class AuthUtils
    {
        public static LoginUser loginUser
        {
            get
            {
                IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
                return (_httpContextAccessor.HttpContext.Session.GetString("LoginUser") != null) ?
                            JsonUtils.DeserializeComplexData<LoginUser>(_httpContextAccessor.HttpContext.Session.GetString("LoginUser")) : null;
            }
        }
        public static void Login(LoginUser user)
        {
            IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
            _httpContextAccessor.HttpContext.Session.SetString("LoginUser", JsonUtils.SerializeComplexData(user));
        }

        public static Boolean IsHr()
        {
            return loginUser == null ? false : loginUser.RoleId.CompareTo(Guid.Parse(CommonEnums.USER_ROLE_ID.HR)) == 0;
        }

        public static Boolean IsInterviewer()
        {
            return loginUser == null ? false : loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.INTERVIEWER);

        }

        public static Boolean IsUser()
        {
            return loginUser == null ? false : loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER);

        }

        public static Boolean IsAdmin()
        {
            return loginUser == null ? false : loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.ADMINISTRATOR);
        }
    }
}
