using BusinessObject;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DataAccess.Repository.Interface;
using Microsoft.AspNetCore.Authorization;

namespace eRecruitmentAPI.Controllers
{
    [Route("api/skills")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private ISkillRepository skillRepository = new SkillRepository();

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> Get()
        {
            try
            {
                var listAllSkills = await skillRepository.GetAllSkill();
                return Ok(listAllSkills);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }
    }
}
