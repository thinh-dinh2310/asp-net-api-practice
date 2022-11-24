using System;
using System.Net;
using System.Threading.Tasks;
using eRecruitmentAPI.Models;
using eRecruitmentAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IdentityModel.Tokens;
using Utils;

namespace eRecruitmentAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthenticationMiddleware
    {
        JwtToken _jwtToken;

        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next, JwtToken jwtToken)
        {
            _jwtToken = jwtToken;
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            var authorization = httpContext.Request.Headers["Authorization"];
            var token = authorization.ToString().Split(' ').Offset<string>(-1);
            try
            {
                var userData = _jwtToken.ValidateAccessToken(token);
                if (userData != null)
                {
                    Console.WriteLine("Authentication middleware" + userData.DisplayName);
                    httpContext.Items.Add("User", userData);
                }
                return _next(httpContext);
            }
            catch (Exception ex)
            {
                if (typeof(SecurityTokenExpiredException).IsInstanceOfType(ex))
                {
                    var url = httpContext.Request.GetDisplayUrl();
                    if (url.EndsWith("accesstoken"))
                    {
                        return _next(httpContext);
                    }
                }
                return HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (typeof(SecurityTokenExpiredException).IsInstanceOfType(exception))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message,
                });
            }
            else
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = exception.Message,
                });
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationExtensions
    {
        public static IApplicationBuilder useAuthenticationMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
