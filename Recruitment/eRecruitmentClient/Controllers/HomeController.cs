using BusinessObject;
using BusinessObject.DTO;
using eRecruitmentClient.Models;
using eRecruitmentClient.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Utils;
using Utils.Models;

namespace eRecruitmentClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Job(string keywords)
        {
            try
            {
                keywords = String.IsNullOrEmpty(keywords) ? "" : keywords.Trim();
                ViewData["CurrentSearch"] = keywords;
                ViewData["CurrentPage"] = 1;
                string GetListPostsApiUrl = CommonEnums.API_PATH + "odata/Posts" + "?limit=" + 10 + "&offset=" + 0 + "&keywords=" + keywords;
                string res = await HttpUtils.SendGetRequestAsync(GetListPostsApiUrl);
                PaginationResult<PostViewModel> response = HttpUtils.DeserializeResponse<PaginationResult<PostViewModel>>(res);

                string InitCreatePostApiUrl = CommonEnums.API_PATH + "Posts/categories-skills";
                res = await HttpUtils.SendGetRequestAsync(InitCreatePostApiUrl);
                Tuple<List<Category>, List<Skill>, List<Location>, List<Level>> data = HttpUtils.DeserializeResponse<Tuple<List<Category>, List<Skill>, List<Location>, List<Level>>>(res);
                ViewData["Category"] = data.Item1;
                ViewData["Skills"] = data.Item2;
                ViewData["Locations"] = data.Item3;
                ViewData["Levels"] = data.Item4;
                return View("Job", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> JobPaging(int limit, int offset, string keywords, string category, string locations, string skills, string levels)
        {
            try
            {
                ViewData["CurrentPage"] = offset + 1;
                string GetListPostsApiUrl = CommonEnums.API_PATH + "odata/Posts"
                    + "?limit=" + limit
                    + "&offset=" + offset
                    + "&keywords=" + keywords
                    + "&category=" + category
                    + "&locations=" + locations
                    + "&skills=" + skills
                    + "&levels=" + levels;
                string res = await HttpUtils.SendGetRequestAsync(GetListPostsApiUrl);
                PaginationResult<PostViewModel> response = HttpUtils.DeserializeResponse<PaginationResult<PostViewModel>>(res);
                return PartialView("~/Views/Home/partial/_JobPartial.cshtml", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> JobDetails(string id)
        {
            try
            {
                MissingSkillWithPVM md = new MissingSkillWithPVM();
                string GetAPostUrl = CommonEnums.API_PATH + "Posts/vm/" + id.ToString();
                string res = await HttpUtils.SendGetRequestAsync(GetAPostUrl);
                PostViewModel post = HttpUtils.DeserializeResponse<PostViewModel>(res);
                string decode = HttpUtility.HtmlDecode(post.Description);
                post.Description = decode;
                md.pvm = post;
                if (AuthUtils.loginUser != null)
                {
                    if (AuthUtils.loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER))
                    {
                        bool canApply = false;
                        bool canRevoke = false;

                        string getMissingSkillUrl = CommonEnums.API_PATH + "account/compareSkill/" + id.ToString() + "/" + AuthUtils.loginUser.Id;
                        string res2 = await HttpUtils.SendGetRequestAsync(getMissingSkillUrl);
                        List<Skill> missingSkill = HttpUtils.DeserializeResponse<List<Skill>>(res2);

                        md.missingSkill = missingSkill;
                        if (post.Status == CommonEnums.POST_STATUS.Available)
                        {
                            string GetFormUrl = CommonEnums.API_PATH + "Forms/user/" + AuthUtils.loginUser.Id.ToString();

                            string resForms = await HttpUtils.SendGetRequestAsync(GetFormUrl);

                            List<ApplicantPost> forms = HttpUtils.DeserializeResponse<List<ApplicantPost>>(resForms);

                            var form = forms.FirstOrDefault(f =>
                                (f.Status == CommonEnums.APPLICATION_FORM_STATUS.Open && f.PostId == post.PostId)
                                || f.Status == CommonEnums.APPLICATION_FORM_STATUS.Approved);

                            if (form == null)
                            {

                                canApply = true;
                            }
                            else
                            {
                                if (form.Status == CommonEnums.APPLICATION_FORM_STATUS.Approved)
                                {
                                    ViewData["FormStatusMessage"] = "You are having an approved form!!!";
                                }
                                else
                                {
                                    ViewData["FormStatusMessage"] = "You have been applying this post!!!";
                                    canRevoke = true;
                                }
                            }
                        }
                        ViewData["CanApply"] = canApply;
                        ViewData["CanRevoke"] = canRevoke;
                    }
                }
                return View("JobDetails", md);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost, ActionName("ApplyPost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyPost([Bind("PostId,Resume,Message")] ApplicationPostForCreationDto applicationPost)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    string CreatePostUrl = CommonEnums.API_PATH + "Posts/application/" + applicationPost.PostId.ToString();
                    Console.WriteLine(CreatePostUrl);
                    string resCreateApplciationForm = await HttpUtils.SendPostRequestApplicationFormAsync(CreatePostUrl, applicationPost);

                    return Json(new { redirectTo = Url.Action("Index", "Home") });
                }
                else
                {
                    string GetAPostUrl = CommonEnums.API_PATH + "Posts/vm/" + applicationPost.PostId.ToString();

                    string res = await HttpUtils.SendGetRequestAsync(GetAPostUrl);
                    PostViewModel post = HttpUtils.DeserializeResponse<PostViewModel>(res);
                    ViewData["Post"] = post;
                    return PartialView("_ApplicationFormModal", new ApplicationPostForCreationDto());
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at ApplyPost: ", ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return Json(new { redirectTo = Url.Action("Index", "Home") });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LoginPage()
        {
            ViewBag.Message = TempData["Message"];
            if (AuthUtils.loginUser != null)
            {
                return View("Index");
            }
            return View("Login");
        }

        public IActionResult RegisterPage()
        {
            ViewBag.Message = TempData["Message"];
            if (AuthUtils.loginUser != null)
            {
                return View("Home");
            }
            return View("Register");
        }

        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                string LoginApiUrl = CommonEnums.API_PATH + "auth/signin";
                LoginUserRequest loginObject = new LoginUserRequest()
                {
                    Email = email,
                    Password = password,
                };

                string res = await HttpUtils.SendPostRequestAsync<LoginUserRequest>(LoginApiUrl, loginObject);

                LoginUser loginUser = HttpUtils.DeserializeResponse<LoginUser>(res);

                if (loginUser != null)
                {
                    AuthUtils.Login(loginUser);
                }
                else
                {
                    return Json(new { url = Url.Action("LoginPage", "Home") });
                }

                if (loginUser.RoleName == CommonEnums.USER_ROLE.ADMINISTRATOR)
                {
                    return Json(new { url = Url.Action("Index", "Account") });
                }

                return Json(new { url = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("~/Views/Home/partial/_LoginError.cshtml");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                string RegisterApiUrl = CommonEnums.API_PATH + "auth/signup";
                if (string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception("Invalid email!");
                }
                user.Id = Guid.NewGuid();
                user.RoleId = Guid.Parse(CommonEnums.USER_ROLE_ID.USER);
                string res = await HttpUtils.SendPostRequestAsync<User>(RegisterApiUrl, user);
                return RedirectToAction("LoginPage");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("RegisterPage");
            }
        }

        public async Task<ActionResult> Logout()
        {
            LoginUser loginUser = AuthUtils.loginUser;
            if (loginUser != null)
            {
                string LogutApiUrl = CommonEnums.API_PATH + "auth/signout";

                await HttpUtils.SendPostRequestAsync<string>(LogutApiUrl, loginUser.RefreshToken);
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> TestAuth()
        {
            string msg = "Ok";
            try
            {
                string TestApiUrl = CommonEnums.API_PATH + "auth/auth-test";

                await HttpUtils.SendGetRequestAsync(TestApiUrl);
            }

            catch (Exception e)
            {
                msg = e.Message;
            }
            ViewBag.Message = msg;
            return View("Index");
        }
    }
}
