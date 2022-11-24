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
using DataAccess.Repository.Interface;
using BusinessObject.DTO;
using System.IO;
using Utils.Models;

namespace eRecruitmentAPI.Controllers
{
    [Route("api/Interviews")]
    [ApiController]
    [Authorize]
    public class InterviewsController : ControllerBase
    {
        IInterviewRepository interviewRepo;

        public InterviewsController()
        {
            interviewRepo = new InterviewRepository();
        }

        [HttpGet]
        public async Task<ActionResult<List<Interview>>> GetListAllInterviews()
        {
            var listInterviews = await interviewRepo.GetAllInterview();
            return Ok(listInterviews);
        }

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<List<Interview>>> GetInterviewsOfAPost(Guid postId)
        {
            var listInterviews = await interviewRepo.GetAllInterviewByPostId(postId);
            return Ok(listInterviews);
        }

        [HttpGet("applicant/{applicantId}")]
        public async Task<ActionResult<List<Interview>>> GetInterviewsByApplicantId(Guid applicantid)
        {
            var listInterviews = await interviewRepo.GetAllInterviewByApplicantId(applicantid);
            return Ok(listInterviews);
        }
        
        [HttpGet("interviewer/{interviewerId}")]
        public async Task<ActionResult<List<Interview>>> GetInterviewsByInterviewerId(Guid interviewerId)
        {
            var listInterviews = await interviewRepo.GetAllInterviewByInterviewerId(interviewerId);
            return Ok(listInterviews);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnInterview([FromBody] Interview interview)
        {
            try
            {
                DaoResponse<string> res = await interviewRepo.CreateInterview(interview.InterviewerId, interview.StartDateTime, interview.EndDateTime, interview.PostId, interview.ApplicantId);
                if (res.IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception(res.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        
        [HttpPut]
        public async Task<IActionResult> UpdateInterview([FromBody] FeedbackDTO interview)
        {
            try
            {
                DaoResponse<string> res = await interviewRepo.UpdateInterview(interview.InterviewerId, interview.PostId, interview.ApplicantId, interview.Round, interview.StartDateTime, interview.EndDateTime, interview.Feedback, interview.Result);
                if (res.IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception(res.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        
        [HttpDelete("{postId}/{applicantId}/{round}")]
        public async Task<IActionResult> DeleteInterview(Guid postId, Guid applicantId, int round)
        {
            try
            {
                DaoResponse<string> res = await interviewRepo.DeleteInterviewById(postId, applicantId, round);
                if (res.IsSuccess)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception(res.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
        [HttpGet("{postId}/{applicantId}/{round}")]
        public async Task<IActionResult> GetInterviewByApplicantAndRound(Guid postId, Guid applicantId, int round)
        {
            try
            {
                Interview res = await interviewRepo.GetInterviewByPostIdApplicantIdAndRound(postId, applicantId, round);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }

        [HttpGet("approved-forms")]
        public async Task<IActionResult> GetListApprovedFormWithInterview()
        {
            try
            {
                var res = await interviewRepo.GetApprovedFormWithInterviewStatus();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
        [HttpGet("editable")]
        public async Task<ActionResult<List<Interview>>> GetListEditableInterviews()
        {
            try
            {
                var listInterviews = await interviewRepo.GetListEditableInterviews();
                return Ok(listInterviews);
            }
            catch(Exception ex)
            {
                return Problem(detail: ex.Message);
            }
            
        }
    }
}
