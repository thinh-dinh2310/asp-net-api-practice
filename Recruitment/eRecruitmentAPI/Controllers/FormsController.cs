using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DataAccess.Repository.Interface;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;
using eRecruitmentAPI.Services;
using Utils;
using eRecruitmentAPI.Filters;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace eRecruitmentAPI.Controllers
{
    [Route("api/Forms")]
    [ApiController]
    [Authorize]
    public class FormsController : ControllerBase
    {
        private IFormRepository formRepository = new FormRepository();
        public FormsController()
        {
        }
        [HttpGet("{postId}")]
        public async Task<ActionResult<IEnumerable<ApplicantPost>>> GetFormsByPostId(Guid postId, [FromQuery] int offset, [FromQuery] int limit, [FromQuery] int status)
        {
            try
            {
                var listAllForms = await formRepository.GetAllApplicationFormOfOnePost(postId, offset, limit, status);
                return Ok(listAllForms);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [HttpGet("detail/{postId}/{userId}/{count}")]
        public async Task<ActionResult<ApplicantPost>> GetFormsByPK(Guid postId, Guid userId, int count)
        {
            try
            {
                User loginUser = (User)HttpContext.Items["User"];
                if (loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER) && loginUser.Id != userId)
                {
                    return Forbid();
                }
                var form = formRepository.GetApplicationPostDetailByPK(postId, userId, count);
                return Ok(form);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [HttpGet("{postId}/{userId}")]
        public async Task<ActionResult<IEnumerable<ApplicantPost>>> GetFormsByPostIdAndUserId(Guid postId, Guid userId, [FromQuery] int offset, [FromQuery] int limit, [FromQuery] int status)
        {
            try
            {
                User loginUser = (User)HttpContext.Items["User"];
                if (loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER) && loginUser.Id != userId)
                {
                    return Forbid();
                }
                var listAllForms = await formRepository.GetAllApplicationFormOfOnePost(postId, offset, limit, status);
                return Ok(listAllForms);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [HttpPatch("{postId}/{userId}")]
        public async Task<ActionResult> PatchForm(Guid postId, Guid userId, [FromBody] JObject formEntry)
        {
            try
            {
                if (formEntry["status"] != null)
                {
                    int status = (int)formEntry["status"];
                    if (status == CommonEnums.APPLICATION_FORM_STATUS.Approved)
                    {
                        await formRepository.ApproveAForm(postId, userId);
                    }
                    if (status == CommonEnums.APPLICATION_FORM_STATUS.Rejected)
                    {
                        await formRepository.RejectAForm(postId, userId);
                    }
                    if (status == CommonEnums.APPLICATION_FORM_STATUS.Revoked)
                    {
                        await formRepository.RevokeAForm(postId, userId);
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }


        [HttpGet("user/{applicantId}")]
        public async Task<ActionResult<IEnumerable<ApplicantPost>>> GetFormsByApplycantId(Guid applicantId)
        {
            try
            {
                User loginUser = (User)HttpContext.Items["User"];
                if (loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER) && applicantId != loginUser.Id)
                {
                    return Forbid();
                }
                var listAllForms = await formRepository.GetListAllFormsOfAnUser(applicantId);
                return Ok(listAllForms.ToList());
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }
    }
}
