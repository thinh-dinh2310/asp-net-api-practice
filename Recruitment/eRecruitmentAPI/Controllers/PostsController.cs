using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Collections.Generic;
using System.Security.Policy;
using System;
using Microsoft.AspNetCore.Authorization;
using BusinessObject;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Results;
using System.Linq;
using BusinessObject.DTO;
using Utils;
using eRecruitmentAPI.Filters;
using eRecruitmentAPI.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace eRecruitmentAPI.Controllers
{
    public class PostsController : ODataController
    {
        private IPostRepository postRepository = new PostRepository();
        private IFormRepository formRepository = new FormRepository();

        [HttpGet]
        public async Task<ActionResult<PaginationResult<PostViewModel>>> Get([FromQuery] int offset,
            [FromQuery] int limit,
            [FromQuery] string locations,
            [FromQuery] string category,
            [FromQuery] string skills,
            [FromQuery] string levels,
            [FromQuery] string keywords)
        {
            try
            {
                var listPosts = await postRepository.GetAllPosts(offset, limit, locations, category, skills, levels, keywords);
                return Ok(listPosts);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [Authorized(CommonEnums.USER_ROLE_ID.HR + "," + CommonEnums.USER_ROLE_ID.ADMINISTRATOR)]
        public ActionResult Post([FromBody] PostForCreationDto post)
        {
            try
            {
                postRepository.CreatePost(post.CategoryId, post.Title, post.Description, post.PostLocationsIds, post.PostSkillsIds, post.LevelId);
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [HttpGet("/api/Posts/{key}")]
        public async Task<ActionResult<Post>> Get(Guid key)
        {
            try
            {
                Post result = await postRepository.GetPostByIdAsync(key);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }
        [Authorized(CommonEnums.USER_ROLE_ID.HR + "," + CommonEnums.USER_ROLE_ID.ADMINISTRATOR)]
        [HttpDelete("/api/Posts/{key}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid key)
        {
            try
            {
                await postRepository.UpdateStatusPost(key, CommonEnums.POST_STATUS.Deleted);
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }
        [HttpGet("/api/Posts/vm/{key}")]
        public async Task<ActionResult<Post>> GetVM(Guid key)
        {
            try
            {
                PostViewModel result = await postRepository.GetPostVMByIdAsync(key);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [HttpGet("/api/Posts/categories-skills")]
        public ActionResult<Tuple<IEnumerable<Category>, IEnumerable<Skill>, IEnumerable<Location>, IEnumerable<Level>>> GetinitialDataForUpdationAndCreation()
        {
            Tuple<IEnumerable<Category>, IEnumerable<Skill>, IEnumerable<Location>, IEnumerable<Level>> result = postRepository.InitDataForCreationOrUpdationPostPage();
            return result;
        }

        [HttpPut("/api/Posts/{key}")]
        public async Task<ActionResult> Put(Guid key, [FromBody] PostForUpdationDto post)
        {
            try
            {
                await postRepository.UpdatePostById(key, post.CategoryId, post.Title, post.Description, post.PostLocationsIds, post.PostSkillsIds);
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message);
            }
        }

        [Authorized(CommonEnums.USER_ROLE_ID.HR + "," + CommonEnums.USER_ROLE_ID.ADMINISTRATOR)]
        [HttpPatch("/api/Posts/{key}")]
        public async Task<ActionResult> PatchPost(Guid key, [FromBody] JObject postEntry)
        {
            try
            {
                if (postEntry["status"] != null)
                {
                    int status = (int)postEntry["status"];
                    DaoResponse<string> res = await postRepository.UpdateStatusPost(key, status);
                    if (res.IsSuccess)
                    {
                        return Ok();
                    } else
                    {
                        throw new Exception(res.ErrorMessage);
                    }
                }
                return Ok();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error at PatchPost: ", e.Message);
                return Problem(detail: e.Message);
            }
        }

        [Authorized(CommonEnums.USER_ROLE_ID.USER)]
        [Consumes("multipart/form-data")]
        [HttpPost("/api/Posts/application/{postId}")]
        public async Task<ActionResult> ApplyOnePost(Guid postId, [FromForm] ApplicationPostForCreationDto applicationPost)
        {
            try
            {
                User loginUser = (User)HttpContext.Items["User"];
                using (var memoryStream = new MemoryStream())
                {
                    await applicationPost.Resume.CopyToAsync(memoryStream);

                    // Upload the file if less than 4 MB
                    if (memoryStream.Length < 4194304)
                    {
                        DaoResponse<string> res = await formRepository.CreateApplicationFormPost(postId, loginUser.Id, applicationPost.Message, memoryStream.ToArray());
                        if (res.IsSuccess)
                        {
                            return Ok();
                        }
                        else
                        {
                            throw new Exception(res.ErrorMessage);
                        }
                    }
                    else
                    {
                        throw new Exception("The file is too large!");
                    }
                }
 
            }
            catch (Exception e)
            {
                Console.WriteLine("Error at ApplyOnePost: ", e.Message);
                return Problem(detail: e.Message);
            }
        }

        [Authorized(CommonEnums.USER_ROLE_ID.USER)]
        [Consumes("multipart/form-data")]
        [HttpPost("/api/Posts/application/resume/{postId}")]
        public async Task<ActionResult> UpdateResumeOfApplicationPost(Guid postId, [FromForm] IFormFile resume)
        {
            try
            {
                User loginUser = (User)HttpContext.Items["User"];
                using (var memoryStream = new MemoryStream())
                {
                    await resume.CopyToAsync(memoryStream);

                    // Upload the file if less than 4 MB
                    if (memoryStream.Length < 4194304)
                    {
                        DaoResponse<string> res = formRepository.UpdateResumeOfApplicationPost(postId, loginUser.Id, memoryStream.ToArray());
                        if (res.IsSuccess)
                        {
                            return Ok();
                        }
                        else
                        {
                            throw new Exception(res.ErrorMessage);
                        }
                    }
                    else
                    {
                        throw new Exception("The file is too large!");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error at CreateOrUpdateResumeOfApplicationPost: ", e.Message);
                return Problem(detail: e.Message);
            }
        }
    }
}
