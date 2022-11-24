using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using eRecruitmentClient.Utils;
using Utils;
using Utils.Models;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using BusinessObject.DTO;
using eRecruitmentClient.Models;
using System.Threading;
using eRecruitmentClient.Services;

namespace eRecruitmentClient
{
    public class InterviewsController : Controller
    {
        private readonly IMailService _mail;

        public InterviewsController(IMailService mail)
        {
            _mail = mail;
        }

        // GET: Interviews
        public async Task<IActionResult> Index()
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiGetInterview = CommonEnums.API_PATH + "Interviews";
                string ApiGetEditableInterview = CommonEnums.API_PATH + "Interviews/editable";
                if (AuthUtils.IsUser())
                {
                    ApiGetInterview += "/applicant" + "/" + loginUser.Id;
                }
                if (AuthUtils.IsInterviewer())
                {
                    ApiGetInterview += "/interviewer" + "/" + loginUser.Id;
                }
                string res = await HttpUtils.SendGetRequestAsync(ApiGetInterview);
                string res2 = await HttpUtils.SendGetRequestAsync(ApiGetEditableInterview);
                List<Interview> listInterviews = HttpUtils.DeserializeResponse<List<Interview>>(res);
                List<Interview> listEditableInterviews = HttpUtils.DeserializeResponse<List<Interview>>(res2);
                ViewData["EditableInterviews"] = listEditableInterviews;
                return View(listInterviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> GetListInterviewsByPostAndApplicant(string postId, string applicantId)
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiGetInterview = CommonEnums.API_PATH + "Interviews";
                string res = await HttpUtils.SendGetRequestAsync(ApiGetInterview);
                List<Interview> listInterviews = HttpUtils.DeserializeResponse<List<Interview>>(res);
                listInterviews = listInterviews.Where(i => i.PostId == Guid.Parse(postId) && i.ApplicantId == Guid.Parse(applicantId)).ToList();
                return View("Index", listInterviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Details(string postId, string applicantId, string round)
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiGetInterview = CommonEnums.API_PATH + "Interviews/" + postId +"/" + applicantId + "/" + round ;
                string res = await HttpUtils.SendGetRequestAsync(ApiGetInterview);
                Interview interview = HttpUtils.DeserializeResponse<Interview>(res);
                return View(interview);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get interview: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Interviews
        public async Task<IActionResult> GetListApprovedForms(string mode)
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiGetForms = CommonEnums.API_PATH + "Interviews/approved-forms";
                string res = await HttpUtils.SendGetRequestAsync(ApiGetForms);
                List<Tuple<ApplicantPost, bool>> listInterviews = HttpUtils.DeserializeResponse<List<Tuple<ApplicantPost, bool>>>(res);
                if (mode == "OpenAndPendingPostOnly")
                {
                    int[] statusList = { CommonEnums.POST_STATUS.Available, CommonEnums.POST_STATUS.Pending };
                    listInterviews = listInterviews.Where(i => statusList.Contains(i.Item1.Post.Status)).ToList();
                }
                ViewBag.CurrentMode = mode;
                return View("ListApprovedForms", listInterviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }


        public async Task<IActionResult> GetListInterviewByPostId(string postId)
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiGetInterview = CommonEnums.API_PATH + "Interviews/post/" + postId;
                
                string res = await HttpUtils.SendGetRequestAsync(ApiGetInterview);
                List<Interview> listInterviews = HttpUtils.DeserializeResponse<List<Interview>>(res);

                if (AuthUtils.IsUser())
                {
                    listInterviews = listInterviews.Where(i => i.ApplicantId == loginUser.Id).ToList();
                }
                if (AuthUtils.IsInterviewer())
                {
                    listInterviews = listInterviews.Where(i => i.InterviewerId == loginUser.Id).ToList();
                }
                ViewData["PostId"] = postId;
                return View("Index", listInterviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at GetListInterviewByPostId: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        //public async Task<IActionResult> Details(Guid? id)
        //{

        //}
        [HttpGet]
        public async Task<IActionResult> Create(string postId, string applicantId)
        {
            if (!AuthUtils.IsHr())
            {
                return Unauthorized();
            }
            try
            {
                string ApiGetListOfInterviewers = CommonEnums.API_PATH + "account/role/" + CommonEnums.USER_ROLE_ID.INTERVIEWER;
                string resInterviewers = await HttpUtils.SendGetRequestAsync(ApiGetListOfInterviewers);
                List<User> listInterviewers = HttpUtils.DeserializeResponse<List<User>>(resInterviewers);
                ViewData["Interviewer"] = new SelectList(listInterviewers, "Id", "Email");

                string GetPostApiUrl = CommonEnums.API_PATH + "Posts/" + postId;
                string resPost = await HttpUtils.SendGetRequestAsync(GetPostApiUrl);
                Post post = HttpUtils.DeserializeResponse<Post>(resPost);
                ViewData["Post"] = post;

                string ApiGetApplicant = CommonEnums.API_PATH + "account/"+ applicantId;
                string resApplicant = await HttpUtils.SendGetRequestAsync(ApiGetApplicant);
                User applicant = HttpUtils.DeserializeResponse<User>(resApplicant);
                ViewData["Applicant"] = applicant;
                return View();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get view create interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Interviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InterviewerId,StartDateTime,EndDateTime,Feedback,Result,Round,PostId,ApplicantId")] Interview interview, string emailTo)
        {
            if (!AuthUtils.IsHr())
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string CreateInterviewUrl = CommonEnums.API_PATH + "Interviews";
                    string resCreateApplciationForm = await HttpUtils.SendPostRequestAsync<Interview>(CreateInterviewUrl, interview);
                    try
                    {
                        await SendMailForm("New interview scheduled", "You have just been scheduled for an interview. <br><br> For more information: visit our website!!!", emailTo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Eror when sendmail: " + ex.Message);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error at Create interviews: " + ex.Message);
                    ViewBag.ErrorMessage = ex.Message;
                    //TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                    //return RedirectToAction("Index", "Home");
                }
            }
            
            try
            {
                string ApiGetListOfInterviewers = CommonEnums.API_PATH + "account/role/" + CommonEnums.USER_ROLE_ID.INTERVIEWER;
                string resInterviewers = await HttpUtils.SendGetRequestAsync(ApiGetListOfInterviewers);
                List<User> listInterviewers = HttpUtils.DeserializeResponse<List<User>>(resInterviewers);
                ViewData["Interviewer"] = new SelectList(listInterviewers, "Id", "Email");

                string GetPostApiUrl = CommonEnums.API_PATH + "Posts/" + interview.PostId;
                string resPost = await HttpUtils.SendGetRequestAsync(GetPostApiUrl);
                Post post = HttpUtils.DeserializeResponse<Post>(resPost);
                ViewData["Post"] = post;

                string ApiGetApplicant = CommonEnums.API_PATH + "account/" + interview.ApplicantId;
                string resApplicant = await HttpUtils.SendGetRequestAsync(ApiGetApplicant);
                User applicant = HttpUtils.DeserializeResponse<User>(resApplicant);
                ViewData["Applicant"] = applicant;
                return View(interview);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get view create interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Interviews/Edit/5
        public async Task<IActionResult> Edit(Guid postId, Guid applicantId, int round, string mode)
        {
            try
            {
                string GetPostApiUrl = CommonEnums.API_PATH + "Posts/" + postId;
                string resPost = await HttpUtils.SendGetRequestAsync(GetPostApiUrl);
                Post post = HttpUtils.DeserializeResponse<Post>(resPost);
                ViewData["Post"] = post;

                string ApiGetApplicant = CommonEnums.API_PATH + "account/" + applicantId;
                string resApplicant = await HttpUtils.SendGetRequestAsync(ApiGetApplicant);
                User applicant = HttpUtils.DeserializeResponse<User>(resApplicant);
                ViewData["Applicant"] = applicant;

                string ApiGetInterview = CommonEnums.API_PATH + "interviews/"+postId+"/" + applicantId +"/" + round;
                string resInterview = await HttpUtils.SendGetRequestAsync(ApiGetInterview);
                Interview interview = HttpUtils.DeserializeResponse<Interview>(resInterview);
                FeedbackDTO feedback = new FeedbackDTO(interview);

                string ApiGetListOfInterviewers = CommonEnums.API_PATH + "account/role/" + CommonEnums.USER_ROLE_ID.INTERVIEWER;
                string resInterviewers = await HttpUtils.SendGetRequestAsync(ApiGetListOfInterviewers);
                List<User> listInterviewers = HttpUtils.DeserializeResponse<List<User>>(resInterviewers);
                ViewData["Interviewer"] = new SelectList(listInterviewers, "Id", "Email");
                ViewBag.Mode = mode;
                return View("EditResult", feedback);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get view edit interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Interviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("InterviewerId,StartDateTime,EndDateTime,Feedback,Result,Round,PostId,ApplicantId")] FeedbackDTO interview, string mode)
        {
            if (interview.EndDateTime <= interview.StartDateTime)
            {
                ViewBag.ErrorMessage = "Cannot re-schedule an interview with EndDateTime <= StartDate in the past!";
            }
            else if (interview.StartDateTime <= DateTime.Now && mode == "Edit")
            {
                ViewBag.ErrorMessage = "Cannot re-schedule an interview with StartDateTime in the past!";
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    
                    if (string.IsNullOrEmpty(interview.Feedback))
                    {
                        interview.Feedback = "";
                    }
                    string UpdaeInterviewUrl = CommonEnums.API_PATH + "Interviews";
                    string resCreateApplciationForm = await HttpUtils.SendPutRequestAsync<FeedbackDTO>(UpdaeInterviewUrl, interview);
                    try
                    {
                        string ApiGetApplicant = CommonEnums.API_PATH + "account/" + interview.ApplicantId.ToString();
                        string resApplicant = await HttpUtils.SendGetRequestAsync(ApiGetApplicant);
                        User applicant = HttpUtils.DeserializeResponse<User>(resApplicant);
                        string emailTo = applicant.Email;
                        if (mode == "Edit")
                        {
                            await SendMailForm("Incomming interview has just been edited", "Your incoming interview has just been re-scheduled. <br><br> For more information: visit our website!!!", emailTo);
                        }
                        if (mode == "Feedback")
                        {
                            await SendMailForm("Feedbacked interview", "Your interview has just been feedbacked. <br><br> For more information: visit our website!!!", emailTo);
                        }
                    } catch (Exception ex)
                    {
                        Console.WriteLine("Eror when sendmail: " + ex.Message);
                    }
                    
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error at Create interviews: " + ex.Message);
                    ViewBag.ErrorMessage = ex.Message;
                    //TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                    //return RedirectToAction("Index", "Home");
                }
            }

            try
            {
                string ApiGetListOfInterviewers = CommonEnums.API_PATH + "account/role/" + CommonEnums.USER_ROLE_ID.INTERVIEWER;
                string resInterviewers = await HttpUtils.SendGetRequestAsync(ApiGetListOfInterviewers);
                List<User> listInterviewers = HttpUtils.DeserializeResponse<List<User>>(resInterviewers);
                ViewData["Interviewer"] = new SelectList(listInterviewers, "Id", "Email");

                string GetPostApiUrl = CommonEnums.API_PATH + "Posts/" + interview.PostId;
                string resPost = await HttpUtils.SendGetRequestAsync(GetPostApiUrl);
                Post post = HttpUtils.DeserializeResponse<Post>(resPost);
                ViewData["Post"] = post;

                string ApiGetApplicant = CommonEnums.API_PATH + "account/" + interview.ApplicantId;
                string resApplicant = await HttpUtils.SendGetRequestAsync(ApiGetApplicant);
                User applicant = HttpUtils.DeserializeResponse<User>(resApplicant);
                ViewData["Applicant"] = applicant;
                ViewBag.Mode = mode;

                return View("EditResult", interview);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get view create interviews: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Interviews/Delete/5
        public async Task<IActionResult> Delete(string postId, string applicantId, string round)
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiGetInterview = CommonEnums.API_PATH + "Interviews/" + postId + "/" + applicantId + "/" + round;
                string res = await HttpUtils.SendGetRequestAsync(ApiGetInterview);
                Interview interview = HttpUtils.DeserializeResponse<Interview>(res);
                return View(interview);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get delete view interview: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Interviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string postId, string applicantId, string round)
        {
            try
            {
                LoginUser loginUser = AuthUtils.loginUser;
                string ApiDeleteInterview = CommonEnums.API_PATH + "Interviews/" + postId + "/" + applicantId + "/" + round;
                string res = await HttpUtils.SendDeleteRequestAsync(ApiDeleteInterview);
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at delete interview: " + ex.Message);
                TempData["Message"] = ex.Message;
                return RedirectToAction("Delete", new { postId, applicantId, round});
            }
        }

        public async Task SendMailForm(string Subject, string Body, string To)
        {
            try
            {
                if (Subject == null || Body == null || To == null)
                {
                    if (Subject == null)
                    {
                        throw new Exception("Subject is required!");
                    }
                    if (Body == null)
                    {
                        throw new Exception("Body is required!");
                    }
                }
                else
                {
                    MailData mailData = new MailData(Subject, Body, To, AuthUtils.loginUser.Email);
                    bool result = await _mail.SendAsync(mailData, new CancellationToken());
                    if (!result)
                    {
                        throw new Exception("Sendmail to " + To + "with subject " + Subject +" with body " + Body + " failed!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get form's detail: " + ex.Message);
            }
        }
    }
}
