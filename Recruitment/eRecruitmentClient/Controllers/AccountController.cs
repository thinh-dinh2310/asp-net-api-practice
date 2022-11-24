using BusinessObject;
using BusinessObject.DTO;
using eRecruitmentClient.Models;
using eRecruitmentClient.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utils;

namespace eRecruitmentClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private string AccountApiUrl = "";
        private List<Role> listRole = null;
        private readonly IHostingEnvironment hostingEnvironment = null;

        public AccountController(ILogger<AccountController> logger, 
            IHostingEnvironment _hostingEnvironment)
        {
            _logger = logger;
            hostingEnvironment = _hostingEnvironment;

            /* 
             * Create temp list which is shown in combobox
             */
            listRole = new List<Role>
            {
                new Role{RoleId = Guid.Parse(CommonEnums.USER_ROLE_ID.USER), RoleName = CommonEnums.USER_ROLE.USER},
                new Role{RoleId = Guid.Parse(CommonEnums.USER_ROLE_ID.HR), RoleName = CommonEnums.USER_ROLE.HR},
                new Role{RoleId = Guid.Parse(CommonEnums.USER_ROLE_ID.INTERVIEWER), RoleName = CommonEnums.USER_ROLE.INTERVIEWER}
            };
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["CurrentPage"] = 1;
                string getListAccountUrl = CommonEnums.API_PATH + "account"
                    + "?limit=" + 7
                    + "&offset=" + 0
                    + "&keywords=";
                string res = await HttpUtils.SendGetRequestAsync(getListAccountUrl);
                PaginationResult<User> listAccount = HttpUtils.DeserializeResponse<PaginationResult<User>>(res);
                return View(listAccount);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> IndexPaging(int limit, int offset, string keywords)
        {
            try
            {
                ViewData["CurrentPage"] = offset + 1;
                string getListAccountUrl = CommonEnums.API_PATH + "account"
                    + "?limit=" + limit
                    + "&offset=" + offset
                    + "&keywords=" + keywords;
                string res = await HttpUtils.SendGetRequestAsync(getListAccountUrl);
                PaginationResult<User> response = HttpUtils.DeserializeResponse<PaginationResult<User>>(res);
                return PartialView("~/Views/Account/_AccountIndexPartial.cshtml", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        //GET: Account
        public async Task<IActionResult> Create()
        {
            ViewBag.Message = TempData["Message"];
            AccountApiUrl = CommonEnums.API_PATH + "account";
            await HttpUtils.SendGetRequestAsync(AccountApiUrl);
            ViewData["RoleId"] = new SelectList(listRole, "RoleId", "RoleName");
            return View();
        }

        //POST: 
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                string AccountApiUrl = CommonEnums.API_PATH + "account";
                if (string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception("Invalid email!");
                }
                user.Id = Guid.NewGuid();
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                user.RoleId = user.RoleId;        
                string res = await HttpUtils.SendPostRequestAsync<User>(AccountApiUrl, user);
                TempData["Message"] = "Successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        //Get: Update account
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Update(Guid? id)
        {
            ViewBag.Message = TempData["Message"];
            AccountApiUrl = CommonEnums.API_PATH + "account/" + id.ToString();
            string strDataRes = await HttpUtils.SendGetRequestAsync(AccountApiUrl);
            User AccountUser = HttpUtils.DeserializeResponse<User>(strDataRes);
            ViewData["RoleId"] = new SelectList(listRole, "RoleId", "RoleName");
            return View(AccountUser);
        }

        //Post: Update account
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Update(User user)
        {
            try
            {
                string AccountApiUrl = CommonEnums.API_PATH + "account";
                if (string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception("Invalid email!");
                }
                string res = await HttpUtils.SendPutRequestAsync<User>(AccountApiUrl, user);
                TempData["Message"] = "Successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Update");
            }
        }


        //Get: Delete account
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            ViewBag.Message = TempData["Message"];
            AccountApiUrl = CommonEnums.API_PATH + "account/" + id.ToString();
            string strDataRes = await HttpUtils.SendGetRequestAsync(AccountApiUrl);
            User AccountUser = HttpUtils.DeserializeResponse<User>(strDataRes);
            ViewData["RoleId"] = new SelectList(listRole, "RoleId", "RoleName");
            return View(AccountUser);
        }


        //Post: Delete account
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Delete(User user)
        {
            try
            {
                string AccountApiUrl = CommonEnums.API_PATH + "account/" + user.Id.ToString();
                string res = await HttpUtils.SendDeleteRequestAsync(AccountApiUrl);
                TempData["SuccessMessage"] = "Delete successfully";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        //Get: Profile
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ViewBag.Message = TempData["Message"];
            AccountApiUrl = CommonEnums.API_PATH + "account/" + AuthUtils.loginUser.Id.ToString();
            string strDataRes = await HttpUtils.SendGetRequestAsync(AccountApiUrl);
            User AccountUser = HttpUtils.DeserializeResponse<User>(strDataRes);
            ProfileViewModel profileViewModel = new ProfileViewModel();
            profileViewModel.userView = AccountUser;

            string GetListPostsApiUrl = CommonEnums.API_PATH + "Skills";
            string res = await HttpUtils.SendGetRequestAsync(GetListPostsApiUrl);
            IEnumerable<Skill> response = HttpUtils.DeserializeResponse<IEnumerable<Skill>> (res);
            ViewData["Skills"] = new SelectList(response, "SkillsId", "SkillName");
            return View(profileViewModel);
        }


        //Post: Update account
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Profile(ProfileViewModel user)
        {
            try
            {
                string AccountApiUrl = CommonEnums.API_PATH + "account";
                if (string.IsNullOrEmpty(user.userView.Email))
                {
                    throw new Exception("Invalid email!");
                }

                if (user.ResumeFileUpload != null)
                {
                    var memoryStream = new MemoryStream();
                    user.ResumeFileUpload.CopyTo(memoryStream);
                    user.userView.Resume = memoryStream.ToArray();
                }

                if(user.UserSkillsIds != null)
                {
                    foreach (var item in user.UserSkillsIds)
                    {
                        UserSkill us = new UserSkill();
                        us.UsersId = user.userView.Id;
                        us.SkillsId = item;
                        user.userView.UserSkills.Add(us);
                    }
                }
                

                string res = await HttpUtils.SendPutRequestAsync<User>(AccountApiUrl, user.userView);

                /* Get user after update */
                AccountApiUrl = CommonEnums.API_PATH + "account/" + user.userView.Id.ToString();
                string strDataRes = await HttpUtils.SendGetRequestAsync(AccountApiUrl);
                User AccountUser = HttpUtils.DeserializeResponse<User>(strDataRes);
                user.userView = AccountUser;

                /* Get list skill */
                string GetListPostsApiUrl = CommonEnums.API_PATH + "Skills";
                string resSkill = await HttpUtils.SendGetRequestAsync(GetListPostsApiUrl);
                IEnumerable<Skill> response = HttpUtils.DeserializeResponse<IEnumerable<Skill>>(resSkill);
                ViewData["Skills"] = new SelectList(response, "SkillsId", "SkillName");
                ViewData["Message"] = "Successfully!";
                return View("Profile",  user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Profile");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> DeleteSkill(Guid skillsId, Guid usersId)
        {
            try
            {
                string AccountApiUrl = CommonEnums.API_PATH + "account/userskill";
                UserSkill us = new UserSkill();
                us.SkillsId = skillsId;
                us.UsersId = usersId;
                string res = await HttpUtils.SendPostRequestAsync<UserSkill>(AccountApiUrl, us);
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Profile");
            }
        }

        public async Task<IActionResult> DownloadResumeFile(string UserID)
        {
            AccountApiUrl = CommonEnums.API_PATH + "account/" + AuthUtils.loginUser.Id.ToString();
            string getAllSkillUrl = CommonEnums.API_PATH + "odata/Skills";
            string strDataRes = await HttpUtils.SendGetRequestAsync(AccountApiUrl);
            User AccountUser = HttpUtils.DeserializeResponse<User>(strDataRes);
            return File(AccountUser.Resume, "application/pdf");
        }

    }
}
