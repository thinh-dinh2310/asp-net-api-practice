using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace eRecruitmentAPI.Filters
{
    public class Authorized : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string allowRoles;
        public Authorized(string roles)
        {
            this.allowRoles = roles;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            User user = (User)context.HttpContext.Items["User"];
            Boolean check = false;
            if (user != null)
            {
                var listRole = allowRoles.Split(',');
                foreach (string role in listRole)
                {
                    if (user.RoleId.ToString().ToLower() == role.ToLower())
                    {
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
            return;
        }
    }
}
