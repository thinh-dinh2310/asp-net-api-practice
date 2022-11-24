using BusinessObject;
using BusinessObject.DTO;
using eRecruitmentClient.Models;
using eRecruitmentClient.Services;
using eRecruitmentClient.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Utils;
using Utils.Models;

namespace eRecruitmentClient.Controllers
{
    public class FormsController : Controller
    {
        private readonly ILogger<FormsController> _logger;
        private readonly IMailService _mail;

        public FormsController(ILogger<FormsController> logger, IMailService mail)
        {
            _logger = logger;
            _mail = mail;
        }

        [HttpGet, ActionName("Index")]
        // list of forms of a post
        public async Task<IActionResult> Index(Guid postId, int offset, int limit, int status)
        {
            try
            {
                APWithMissingSkill aPWithMissingSkill = new APWithMissingSkill();
                
                var user = AuthUtils.loginUser;
                string GetAPostUrl = CommonEnums.API_PATH + "Posts/vm/" + postId.ToString();

                string res = await HttpUtils.SendGetRequestAsync(GetAPostUrl);

                PostViewModel post = HttpUtils.DeserializeResponse<PostViewModel>(res);
                ViewData["CurrentPost"] = post;
                if(limit == 0)
                {
                    limit = 7;
                }

                var GetListFormsOfPostUrlAPI = CommonEnums.API_PATH + "Forms/" + postId.ToString()
                    + "?offset=" + offset
                    + "&limit=" + limit
                    + "&status=" + status;
                ViewData["CurrentPage"] = offset;
                ViewData["CurrentStatus"] = status;
                string strDataRes = await HttpUtils.SendGetRequestAsync(GetListFormsOfPostUrlAPI);
                PaginationResult<ApplicantPost> listForm = HttpUtils.DeserializeResponse<PaginationResult<ApplicantPost>>(strDataRes);
                aPWithMissingSkill.ap = listForm;

                string getMissingSkillUrl = CommonEnums.API_PATH + "account/compareSkill/" + postId.ToString();
                string res2 = await HttpUtils.SendGetRequestAsync(getMissingSkillUrl);
                List<UserSkillWithResult> missingSkill = HttpUtils.DeserializeResponse<List<UserSkillWithResult>>(res2);

                aPWithMissingSkill.userMissingSkills = missingSkill;
                Console.WriteLine(aPWithMissingSkill.userMissingSkills.Count());
                foreach (var item in listForm.data)
                {
                    
                }
                if (user.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER))
                {
                    listForm.data = listForm.data.Where(form => form.ApplicantId == user.Id).ToList();
                }
                return View(aPWithMissingSkill);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost, ActionName("Status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid postId, Guid userId, int status)
        {
            try
            {
                string PatchStatusFormUrl = CommonEnums.API_PATH + "Forms/" + postId.ToString() + "/" + userId.ToString();
                var payload = new
                {
                    status = status,
                };
                JObject body = JObject.FromObject(payload);
                string res = await HttpUtils.SendPatchRequestAsync<JObject>(PatchStatusFormUrl, body);
                await Task.Delay(1000);
                return RedirectToAction("Index", new { postId = postId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at UpdateStatus: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet, ActionName("RevokeForm")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeForm(Guid postId)
        {
            try
            {
                if (AuthUtils.loginUser == null)
                {
                    return Unauthorized();
                }
                string PatchStatusFormUrl = CommonEnums.API_PATH + "Forms/" + postId.ToString() + "/" + AuthUtils.loginUser.Id.ToString();
                var payload = new
                {
                    status = CommonEnums.APPLICATION_FORM_STATUS.Revoked,
                };
                JObject body = JObject.FromObject(payload);
                string res = await HttpUtils.SendPatchRequestAsync<JObject>(PatchStatusFormUrl, body);
                await Task.Delay(1000);
                return RedirectToAction("Index", "Posts");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at RevokeForm: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet, ActionName("Detail")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(Guid postId, Guid applicantId, int count)
        {
            try
            {
                if (AuthUtils.loginUser == null)
                {
                    return Unauthorized();
                }
                string GetFormUrl = CommonEnums.API_PATH + "Forms/detail/" + postId.ToString() + "/" + applicantId.ToString() + "/" + count;
                string res = await HttpUtils.SendGetRequestAsync(GetFormUrl);
                ApplicantPost form = HttpUtils.DeserializeResponse<ApplicantPost>(res);
                return View("FormDetail", form);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get form's detail: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet, ActionName("DownloadResume")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadResume(Guid postId, Guid applicantId, int count)
        {
            try
            {
                if (AuthUtils.loginUser == null)
                {
                    return Unauthorized();
                }
                string GetFormUrl = CommonEnums.API_PATH + "Forms/detail/" + postId.ToString() + "/" + applicantId.ToString() + "/" + count;
                string res = await HttpUtils.SendGetRequestAsync(GetFormUrl);
                ApplicantPost form = HttpUtils.DeserializeResponse<ApplicantPost>(res);
                return File(form.Resume, "application/pdf", "resume_" + postId + "_" + applicantId + "_" + count + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get form's detail: " + ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet, ActionName("OpenSendMailFormModal")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> OpenSendMailFormModal(string receiverEmail, string defaultSubject)
        {
            try
            {
                ViewData["ReceiverEmail"] = receiverEmail;
                ViewData["DefaultSubject"] = defaultSubject;
                return PartialView("~/Views/Forms/partial/_SendMailForm.cshtml");
            }
            catch (Exception ex)
            {
                return Json(new { redirectTo = Url.Action("Index") });
            }
                
        }

        [HttpPost, ActionName("SendMailForm")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailForm(string Subject, string Body, string To)
        {
            try
            {
                if (Subject == null || Body == null || To == null)
                {
                    if(Subject == null)
                    {
                        ViewBag.MessageSubject = "Subject is required!";
                    }
                    if (Body == null)
                    {
                        ViewBag.MessageBody = "Body is required!";
                    }
                    ViewData["ReceiverEmail"] = To;
                    ViewData["DefaultSubject"] = Subject;
                    return PartialView("~/Views/Forms/partial/_SendMailForm.cshtml");
                    
                }
                else
                {
                    MailData mailData = new MailData(Subject, Body, To, AuthUtils.loginUser.Email);
                    bool result = await _mail.SendAsync(mailData, new CancellationToken());

                    return Json(new { redirectTo = Url.Action("Index") });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at get form's detail: " + ex.Message);
                //TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return Json(new { redirectTo = Url.Action("Index") });
            }
        }

        [HttpGet, ActionName("IndexApplicant")]
        // list of forms of a post
        public async Task<IActionResult> AllFormsOfCurrentApplicant()
        {
            try
            {
                if (!AuthUtils.IsUser())
                {
                    return Forbid();
                }
                var user = AuthUtils.loginUser;

                string GetFormUrl = CommonEnums.API_PATH + "Forms/user/" + user.Id.ToString();

                string res = await HttpUtils.SendGetRequestAsync(GetFormUrl);

                List<ApplicantPost> listForm = HttpUtils.DeserializeResponse<List<ApplicantPost>>(res);
                
                return View(listForm);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
