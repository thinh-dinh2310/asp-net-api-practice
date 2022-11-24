using BusinessObject;
using BusinessObject.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DataAccess.DAO
{
    public class PostsDAO
    {
        private static PostsDAO instance = null;
        private static readonly object instanceLock = new object();
        private PostsDAO() { }

        public static PostsDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new PostsDAO();
                    }
                }
                return instance;
            }
        }

        public async Task<PaginationResult<PostViewModel>> GetAllPosts(int offset = 0, int limit = 10,
            string locations = "",
            string category = "",
            string skills = "",
            string levels = "",
            string keywords = "")
        {
            PaginationResult<PostViewModel> response = new PaginationResult<PostViewModel>();
            List<PostViewModel> list = new List<PostViewModel>();
            try
            {
                var context = new eRecruitmentContext();
                List<Guid> locationIds = String.IsNullOrEmpty(locations) 
                    ? context.Locations.Select(e => new Guid(e.LocationId.ToString())).ToList()
                    : locations.Split(new char[] { ' ' }).ToList().Select(l => Guid.Parse(l)).ToList();

                List<Guid> categoriesIds = String.IsNullOrEmpty(category)
                    ? context.Categories.Select(e => new Guid(e.CategoryId.ToString())).ToList()
                    : category.Split(new char[] { ' ' }).ToList().Select(c => Guid.Parse(c)).ToList();

                List<Guid> skillsIds = String.IsNullOrEmpty(skills)
                    ? context.Skills.Select(e => new Guid(e.SkillsId.ToString())).ToList()
                    : skills.Split(new char[] { ' ' }).ToList().Select(s => Guid.Parse(s)).ToList();

                List<Guid> levelsId = String.IsNullOrEmpty(levels)
                    ? context.Levels.Select(e => new Guid(e.LevelId.ToString())).ToList()
                    : levels.Split(new char[] { ' ' }).ToList().Select(lv => Guid.Parse(lv)).ToList();
                keywords = String.IsNullOrEmpty(keywords) ? "" : keywords.Trim();

                var listPost = context.Posts
                    .Include(p => p.Category)
                    .Include(p => p.Level)
                    .Include(p => p.LocationPosts)
                        .ThenInclude(e => e.Location)
                    .Include(p => p.PostSkills)
                        .ThenInclude(e => e.Skills)
                        .Where(c => categoriesIds.Contains(c.CategoryId))
                        .Where(lv => levelsId.Contains(lv.LevelId))
                        .Where(l => l.LocationPosts.Any(x => locationIds.Contains(x.LocationId)))
                        .Where(s => s.PostSkills.Any(x => skillsIds.Contains(x.SkillsId)))
                        .Where(k => k.Title.ToLower().Contains(keywords));

                var convertList = await listPost
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip(offset * limit)
                    .Take(limit).ToListAsync();

                list = convertList.Select(e => new PostViewModel(e)).ToList();

                response.limit = limit;
                response.offset = offset;
                response.totalInPage = list.Count();
                response.totalItems = listPost.Count();
                response.data = list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " Error at GetAllPosts: " + ex.Message);
            }
            return response;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            Post tmp = new Post();
            try
            {
                var context = new eRecruitmentContext();
                tmp = await context.Posts
                    .Include(p => p.Level)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(m => m.PostId == postId);
                tmp.LocationPosts = context.LocationPosts
                        .AsNoTracking()
                        .Include(e => e.Location)
                        .Where(e => e.PostId == tmp.PostId)
                        .ToList();
                tmp.PostSkills = context.PostSkills
                    .AsNoTracking()
                    .Include(e => e.Skills)
                    .Where(e => e.PostId == tmp.PostId)
                    .ToList();
                tmp.ApplicantPosts = context.ApplicantPosts
                    .AsNoTracking()
                    .Include(e => e.Applicant)
                    .Where(e => e.PostId == tmp.PostId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetPostByIdAsync: " + ex.Message);
            }
            return tmp;
        }

        public async Task<PostViewModel> GetPostVMByIdAsync(Guid postId)
        {
            PostViewModel tmp = new PostViewModel();
            try
            {
                var context = new eRecruitmentContext();
                var post = await context.Posts
                   .AsNoTracking()
                   .Include(p => p.Level)
                   .Include(p => p.Category)
                   .Include(p => p.LocationPosts).ThenInclude(e => e.Location)
                   .Include(p => p.PostSkills).ThenInclude(e => e.Skills)
                   .FirstOrDefaultAsync(m => m.PostId == postId);
                tmp = new PostViewModel(post);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetPostByIdAsync: " + ex.Message);
            }
            return tmp;
        }

        public async Task UpdatePostById(Guid postId, Guid categoryId, string title, string description, Guid[] postLocationsIds, Guid[] postSkillsIds)
        {
            try
            {
                var context = new eRecruitmentContext();
                Post Post = await GetPostByIdAsync(postId);
                if (Post == null)
                {
                    throw new Exception("Post not found");
                }
                Post.CategoryId = categoryId;
                Post.Title = title.Trim();
                Post.Description = description.Trim();
                Post.UpdatedAt = DateTime.Now;
                context.RemoveRange(Post.LocationPosts);
                context.RemoveRange(Post.PostSkills);
                List<PostSkill> postSkills = new List<PostSkill>();
                List<LocationPost> postLocations = new List<LocationPost>();

                foreach (Guid skillId in postSkillsIds)
                {
                    var skill = context.Skills.FirstOrDefault(skill => skill.SkillsId == skillId);
                    if (skill == null)
                    {
                        continue;
                    }
                    PostSkill postSkill = new PostSkill()
                    {
                        SkillsId = skillId,
                        PostId = postId,
                    };
                    postSkills.Add(postSkill);
                }
                Post.PostSkills = postSkills;

                foreach (Guid locationId in postLocationsIds)
                {
                    var location = context.Locations.FirstOrDefault(location => location.LocationId == locationId);
                    if (location == null)
                    {
                        continue;
                    }
                    LocationPost LocationPost = new LocationPost()
                    {
                        LocationId = locationId,
                        PostId = postId,
                    };
                    postLocations.Add(LocationPost);
                }
                Post.PostSkills = postSkills;
                Post.LocationPosts = postLocations;
                context.AddRange(Post.LocationPosts);
                context.AddRange(Post.PostSkills);

                context.Posts.Update(Post);
                context.Entry(Post).State = EntityState.Modified;

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Updated Post successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at UpdatePostById: " + ex.Message);
            }
        }

        public async Task<DaoResponse<string>> UpdatePostStatus(Guid PostId, int status)
        {
            try
            {
                var context = new eRecruitmentContext();
                using var transaction = context.Database.BeginTransaction();
                Post post = await context.Posts
                    .Include(p => p.Level)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(m => m.PostId == PostId);
                post.ApplicantPosts = context.ApplicantPosts
                    .Include(e => e.Applicant)
                    .Where(e => e.PostId == PostId)
                    .ToList();
                post.Status = status;
                if (status == CommonEnums.POST_STATUS.Closed)
                {
                    var openForm = post.ApplicantPosts
                        .FirstOrDefault(p => p.PostId == PostId && p.Status == CommonEnums.APPLICATION_FORM_STATUS.Open);
                    // if having a form that is unproccessed => throw error
                    if (openForm != null)
                    {
                        throw new Exception("This post is having at least 1 form that is Open!");
                    }
                    var unfeedbackedInterview = context.Interviews
                        .FirstOrDefault(i => i.PostId == PostId && i.Feedback == "" && i.Result == false);
                    if (unfeedbackedInterview != null)
                    {
                        throw new Exception("This post is having at least 1 interview that is untouched!");
                    }
                    var approvedForm = post.ApplicantPosts
                        .Where(p => p.Status == CommonEnums.APPLICATION_FORM_STATUS.Approved).ToList();
                    foreach (var form in approvedForm)
                    {
                        var pendingFormsOfCurrentApplicant = context.ApplicantPosts.Where(e => e.ApplicantId == form.ApplicantId && e.Status == CommonEnums.APPLICATION_FORM_STATUS.Pending);
                        var latestInterview = context.Interviews.Where(i => i.ApplicantId == form.ApplicantId && i.PostId == PostId).OrderByDescending(i => i.Round).FirstOrDefault();
                        if (latestInterview == null)
                        {
                            throw new Exception("This post is having at least 1 approved applicant who is not having any interview yet!");
                        }
                        if (latestInterview.Result)
                        {
                            form.Status = CommonEnums.APPLICATION_FORM_STATUS.Successfully;
                        }
                        else
                        {
                            form.Status = CommonEnums.APPLICATION_FORM_STATUS.Failed;
                        }
                        foreach (var pendingForm in pendingFormsOfCurrentApplicant)
                        {
                            // pv đậu -> tất cả những Form đã applied đang pending -> batch rejected
                            if (latestInterview.Result)
                            { 
                                pendingForm.Status = CommonEnums.APPLICATION_FORM_STATUS.BatchRejected;
                            } 
                            else
                            {
                                // TH pv rớt và post này đang available -> Open form lại
                                Post targetPost = context.Posts.FirstOrDefault(p => p.PostId == pendingForm.PostId);
                                if (targetPost.Status == CommonEnums.POST_STATUS.Available)
                                {
                                    pendingForm.Status = CommonEnums.APPLICATION_FORM_STATUS.Open;
                                }
                                else
                                {
                                    pendingForm.Status = CommonEnums.APPLICATION_FORM_STATUS.BatchRejected;
                                }
                            }
                            context.ApplicantPosts.Update(pendingForm);
                        }

                    }
                }
                
                context.Posts.Update(post);
                if (context.SaveChanges() > 0)
                {
                    transaction.Commit();
                    return new DaoResponse<string>(true, "");
                }
                throw new Exception("Update status post failed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at UpdatePostStatus: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }
        }   

        public Tuple<IEnumerable<Category>, IEnumerable<Skill>, IEnumerable<Location>, IEnumerable<Level>> InitDataForCreationOrUpdationPostPage()
        {
            IEnumerable<Category> categories = new List<Category>();
            IEnumerable<Skill> skills = new List<Skill>();
            IEnumerable<Location> locations = new List<Location>();
            IEnumerable<Level> levels = new List<Level>();
            try
            {
                var context = new eRecruitmentContext();
                categories = context.Categories.ToList();
                skills = context.Skills.ToList();
                locations = context.Locations.ToList();
                levels = context.Levels.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at UpdatePostStatus: " + ex.Message);
            }
            Tuple<IEnumerable<Category>,
                IEnumerable<Skill>,
                IEnumerable<Location>,
                IEnumerable<Level>> cateAndSkills = new Tuple<IEnumerable<Category>,
                                                    IEnumerable<Skill>,
                                                    IEnumerable<Location>,
                                                    IEnumerable<Level>>(categories, skills, locations, levels);
            return cateAndSkills;
        }

        public void CreatePost(Guid categoryId, string title, string description, Guid[] postLocationsIds, Guid[] postSkillsIds, Guid levelId)
        {
            try
            {
                var context = new eRecruitmentContext();
                Guid postId = Guid.NewGuid();
                Post Post = new Post()
                {
                    PostId = postId,
                    Title = title.Trim(),
                    Description = description.Trim(),
                    Status = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CategoryId = categoryId,
                    LevelId = levelId
                };
                List<PostSkill> postSkills = new List<PostSkill>();
                List<LocationPost> postLocations = new List<LocationPost>();

                foreach (Guid skillId in postSkillsIds)
                {
                    var skill = context.Skills.FirstOrDefault(skill => skill.SkillsId == skillId);
                    if (skill == null)
                    {
                        continue;
                    }
                    PostSkill postSkill = new PostSkill()
                    {
                        SkillsId = skillId,
                        PostId = postId,
                    };
                    postSkills.Add(postSkill);
                }
                Post.PostSkills = postSkills;

                foreach (Guid locationId in postLocationsIds)
                {
                    var location = context.Locations.FirstOrDefault(location => location.LocationId == locationId);
                    if (location == null)
                    {
                        continue;
                    }
                    LocationPost LocationPost = new LocationPost()
                    {
                        LocationId = locationId,
                        PostId = postId,
                    };
                    postLocations.Add(LocationPost);
                }
                Post.PostSkills = postSkills;
                Post.LocationPosts = postLocations;
                context.Posts.Add(Post);
                context.Entry(Post).State = EntityState.Added;

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Created Post successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at CreatePost: " + ex.Message);
            }
        }

    }
}
