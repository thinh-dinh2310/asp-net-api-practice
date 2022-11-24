using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository;
using eRecruitmentAPI.Services;
using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BusinessObject.DTO;

namespace eRecruitmentAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IUserRepository userRepo;


        public AccountController(JwtToken jwtToken)
        {
            userRepo = new UserRepository();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PaginationResult<User>>> GetlistAccount([FromQuery] int offset,
            [FromQuery] int limit,
            [FromQuery] string keywords)
        {
            var listAccount = await userRepo.GetAllUsers(offset, limit, keywords);
            return Ok(listAccount);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByID(string id)
        {
            var account = await userRepo.GetUserById(Guid.Parse(id));
            return Ok(account);
        }

        [AllowAnonymous]
        [HttpGet("compareSkill/{id}")]
        public async Task<ActionResult<List<UserSkillWithResult>>> GetComparedSkill(string id)
        {
            var missing = await userRepo.GetUserComparedSkill(Guid.Parse(id));
            return Ok(missing);
        }

        [AllowAnonymous]
        [HttpGet("compareSkill/{postId}/{userId}")]
        public async Task<ActionResult<List<Skill>>> GetComparedSkill(string postId, string userId)
        {
            var missing = await userRepo.GetOneUsersCompareSkill(Guid.Parse(postId), Guid.Parse(userId));
            return Ok(missing);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] User user)
        {
            try
            {
                User checkUser = await userRepo.GetUserByEmail(user.Email);
                if (checkUser != null)
                {
                    throw new Exception("This email has been registered!");
                }
                userRepo.CreateUser(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateMember([FromBody] User user)
        {
            try
            {
                User checkUser = await userRepo.GetUserByEmail(user.Email);
                if (checkUser != null && checkUser.Id != user.Id)
                {
                    throw new Exception("This email is not exist!");
                }
                userRepo.UpdateUser(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public ActionResult DeleteMember(string id)
        {
            try
            {
                userRepo.DeleteUserById(Guid.Parse(id));
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("userskill")]
        public ActionResult DeleteMemberSkill([FromBody] UserSkill us)
        {
            try
            {
                userRepo.DeleteUserSkill(us);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                return Problem(detail: ex.Message);
            }
        }

        [Authorize]
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<List<User>>> GetAllUsersByRoleId(Guid roleId)
        {
            try
            {
                var listAccount = await userRepo.GetAllUsersByRoleId(roleId);
                return Ok(listAccount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                return Problem(detail: ex.Message);
            }
        }

    }
}
