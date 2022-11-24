using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Utils.Models;
using Utils;
using BusinessObject.DTO;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using eRecruitmentClient.Utils;
using System.Web;

namespace eRecruitmentClient.Controllers
{
    public class PostsController : Controller
    {

        public PostsController()
        {
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["CurrentPage"] = 1;
                string GetListPostsApiUrl = CommonEnums.API_PATH + "odata/Posts" + "?limit=" + 7 + "&offset=" + 0;
                string res = await HttpUtils.SendGetRequestAsync(GetListPostsApiUrl);
                PaginationResult<PostViewModel> response = HttpUtils.DeserializeResponse<PaginationResult<PostViewModel>>(res);

                return View(response);
            }
            catch (Exception ex)
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
                string GetListPostsApiUrl = CommonEnums.API_PATH + "odata/Posts"
                    + "?limit=" + limit
                    + "&offset=" + offset
                    + "&keywords=" + keywords;
                string res = await HttpUtils.SendGetRequestAsync(GetListPostsApiUrl);
                PaginationResult<PostViewModel> response = HttpUtils.DeserializeResponse<PaginationResult<PostViewModel>>(res);

                return PartialView("~/Views/Posts/_JobIndexPartial.cshtml", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                string GetAPostUrl = CommonEnums.API_PATH + "Posts/vm/" + id.ToString();
                string res = await HttpUtils.SendGetRequestAsync(GetAPostUrl);
                PostViewModel post = HttpUtils.DeserializeResponse<PostViewModel>(res);
                string decode = HttpUtility.HtmlDecode(post.Description);
                post.Description = decode;
                if (AuthUtils.loginUser.RoleId == Guid.Parse(CommonEnums.USER_ROLE_ID.USER))
                {
                    bool canApply = false;
                    bool canRevoke = false;
                    if (post.Status == CommonEnums.POST_STATUS.Available)
                    {
                        string GetFormUrl = CommonEnums.API_PATH + "Forms/user/" + AuthUtils.loginUser.Id.ToString();

                        string resForms = await HttpUtils.SendGetRequestAsync(GetFormUrl);

                        List<ApplicantPost> forms = HttpUtils.DeserializeResponse<List<ApplicantPost>>(resForms);

                        var form = forms.FirstOrDefault(f =>
                            (f.Status == CommonEnums.APPLICATION_FORM_STATUS.Open && f.PostId == id)
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
                            } else
                            {
                                ViewData["FormStatusMessage"] = "You have been applying this post!!!";
                                canRevoke = true;
                            }
                        }
                    }
                    ViewData["CanApply"] = canApply;
                    ViewData["CanRevoke"] = canRevoke;
                }

                return View(post);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Posts/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                string InitCreatePostApiUrl = CommonEnums.API_PATH + "Posts/categories-skills";

                string res = await HttpUtils.SendGetRequestAsync(InitCreatePostApiUrl);

                Tuple<List<Category>, List<Skill>, List<Location>, List<Level>> data = HttpUtils.DeserializeResponse<Tuple<List<Category>, List<Skill>, List<Location>, List<Level>>>(res);
                ViewData["Category"] = new SelectList(data.Item1, "CategoryId", "CategoryName");
                ViewData["Skills"] = new SelectList(data.Item2, "SkillsId", "SkillName");
                ViewData["Locations"] = new SelectList(data.Item3, "LocationId", "LocationName");
                ViewData["Levels"] = new SelectList(data.Item4, "LevelId", "LevelName");
                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Title,Description,PostLocationsIds,PostSkillsIds, LevelId")] PostForCreationDto post)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (post.PostSkillsIds == null)
                    {
                        post.PostSkillsIds = new Guid[] {};
                    }
                    if (post.PostLocationsIds == null)
                    {
                        post.PostLocationsIds = new Guid[] { };
                    }
                    string CreateListPostsApiUrl = CommonEnums.API_PATH + "odata/Posts";

                    string res = await HttpUtils.SendPostRequestAsync<PostForCreationDto>(CreateListPostsApiUrl, post);

                    return RedirectToAction("Index");
                } else
                {
                    throw new Exception("Not all fields validated!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong!!! Error: " + ex.Message;
                return RedirectToAction("Create");

            }

        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                string GetListPostApiUrl = CommonEnums.API_PATH + "Posts/" + id.ToString();
                string res = await HttpUtils.SendGetRequestAsync(GetListPostApiUrl);
                Post post = HttpUtils.DeserializeResponse<Post>(res);
                string InitCreatePostApiUrl = CommonEnums.API_PATH + "Posts/categories-skills";
                string initialData = await HttpUtils.SendGetRequestAsync(InitCreatePostApiUrl);
                PostForUpdationDto postupdation = new PostForUpdationDto(post);
                
                Tuple<List<Category>, List<Skill>, List<Location>, List<Level>> data = HttpUtils.DeserializeResponse<Tuple<List<Category>, List<Skill>, List<Location>, List<Level>>>(initialData);

                ViewData["Category"] = new SelectList(data.Item1, "CategoryId", "CategoryName");
                ViewData["Skills"] = new SelectList(data.Item2, "SkillsId", "SkillName");
                ViewData["Locations"] = new SelectList(data.Item3, "LocationId", "LocationName");
                ViewData["Levels"] = new SelectList(data.Item4, "LevelId", "LevelName");
                ViewBag.Message = TempData["Message"];
                return View(postupdation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PostId,CategoryId,Title,Description,PostLocationsIds,PostSkillsIds")] PostForUpdationDto post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            try
            {

                if (ModelState.IsValid)
                {
                    if (post.PostSkillsIds == null)
                    {
                        post.PostSkillsIds = new Guid[] { };
                    }
                    if (post.PostLocationsIds == null)
                    {
                        post.PostLocationsIds = new Guid[] { };
                    }
                    string UpdateListPostsApiUrl = CommonEnums.API_PATH + "odata/Posts/" + id.ToString();

                    string res = await HttpUtils.SendPutRequestAsync<PostForUpdationDto>(UpdateListPostsApiUrl, post);
                    return RedirectToAction("Index");
                } else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction(nameof(Edit));
            }

        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                string GetListPostApiUrl = CommonEnums.API_PATH + "Posts/vm/" + id.ToString();

                string res = await HttpUtils.SendGetRequestAsync(GetListPostApiUrl);

                PostViewModel post = HttpUtils.DeserializeResponse<PostViewModel>(res);

                ViewBag.Message = TempData["Message"];
                return View(post);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                string DeletePostUrl = CommonEnums.API_PATH + "Posts/" + id.ToString();

                string res = await HttpUtils.SendDeleteRequestAsync(DeletePostUrl);
                await Task.Delay(1000);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost, ActionName("Status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, int status)
        {
            try
            {
                string PatchStatusPostUrl = CommonEnums.API_PATH + "Posts/" + id.ToString();
                var payload = new
                {
                    status = status,
                };
                JObject body = JObject.FromObject(payload);
                string res = await HttpUtils.SendPatchRequestAsync<JObject>(PatchStatusPostUrl, body);
                
                await Task.Delay(1000);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["Message"] =  ex.Message;
                return RedirectToAction("Details", new { id = id });
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
                    string resCreateApplciationForm = await HttpUtils.SendPostRequestApplicationFormAsync(CreatePostUrl, applicationPost);

                    return Json(new { redirectTo = Url.Action("Index","Home") });
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

        // GET: Posts/Details/5
        public async Task<IActionResult> FormsPage(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                return RedirectToAction("Index", "Forms", new { postId = id });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> InterviewsPage(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                return RedirectToAction("GetListInterviewByPostId", "Interviews", new { postId = id });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Something went wrong - Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
