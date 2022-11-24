using BusinessObject;
using DataAccess.Repository;
using eRecruitmentAPI.Filters;
using eRecruitmentAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Utils;
using Utils.Models;

namespace eRecruitmentAPI.Controllers.Authentication
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        IUserRepository userRepo;

        private readonly JwtToken _jwtToken;
        public AuthenticationController(JwtToken jwtToken)
        {
            userRepo = new UserRepository();
            _jwtToken = jwtToken;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginUser>> Login([FromBody] LoginUserRequest payload)
        {
            try
            {
                var userDB = await userRepo.GetUserByEmailAndPassword(payload.Email, payload.Password);
                if (userDB == null)
                {
                    throw new Exception("Wrong email or password!");
                }
                string accessToken = _jwtToken.GenerateAccessToken(userDB);
                string refreshToken = _jwtToken.GenerateRefreshToken(userDB);
                userRepo.InsertRefreshToken(refreshToken, userDB.Id);
                LoginUser res = new LoginUser(userDB, accessToken, refreshToken);
                return res;
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [Authorize]
        [HttpPost("accesstoken")]
        public async Task<ActionResult<string>> GenerateAccessTokenFromRefreshToken([FromBody] string refreshToken)
        {
            var authTokenDb = userRepo.GetRefreshToken(refreshToken);
            var userData = _jwtToken.ValidateRefreshToken(refreshToken);

            if (userData == null || authTokenDb == null || authTokenDb.UserId != userData.Id)
            {
                return "";
            }

            var user = await userRepo.GetUserById(userData.Id);
            var newAccessToken = _jwtToken.GenerateAccessToken(user);
            return newAccessToken;
        }

        [Authorize]
        [HttpPost("signout")]
        public IActionResult Logout([FromBody] string refreshToken)
        {
            userRepo.DeleteAuthToken(refreshToken);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            try
            {
                User checkUser = await userRepo.GetUserByEmail(user.Email);
                if (checkUser != null)
                {
                    throw new Exception("This email has been registered!");
                }
                userRepo.RegisterUser(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        [Authorize]
        [HttpGet("auth-test")]
        public async Task<IActionResult> TestAuth()
        {
            return Ok();
        }

        [Authorized(CommonEnums.USER_ROLE_ID.USER + "," + CommonEnums.USER_ROLE_ID.HR)]
        [HttpGet("auth-test-user-hr")]
        public async Task<IActionResult> TestAuthUserHr()
        {
            return Ok();
        }
    }
}
