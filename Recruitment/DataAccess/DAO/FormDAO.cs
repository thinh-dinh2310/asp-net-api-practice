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
    public class FormDAO
    {
        private static FormDAO instance = null;
        private static readonly object instanceLock = new object();
        private FormDAO() { }

        public static FormDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new FormDAO();
                    }
                }
                return instance;
            }
        }

        public async Task<PaginationResult<ApplicantPost>> GetAllApplicationFormOfOnePost(Guid postId, int offset, int limit, int status)
        {
            PaginationResult<ApplicantPost> response = new PaginationResult<ApplicantPost>();
            List<ApplicantPost> list = new List<ApplicantPost>();
            List<int> statuses = new List<int>();
            if (status == 0)
            {
                statuses.Add(1);
                statuses.Add(2);
                statuses.Add(3);
                statuses.Add(4);
                statuses.Add(5);
                statuses.Add(6);
                statuses.Add(7);
                statuses.Add(8);
            }
            else
            {
                statuses.Add(status);
            }
            try
            {
                var context = new eRecruitmentContext();
                list = await context.ApplicantPosts
                    .Include(form => form.Applicant)
                    .Include(form => form.Post)
                    .Where(form => form.PostId == postId)
                    .Where(form => statuses.Contains(form.Status))
                    .Skip(offset * limit)
                    .Take(limit)
                    .Select(form => new ApplicantPost() {
                        PostId = form.PostId,
                        ApplicantId = form.ApplicantId,
                        Message = form.Message,
                        Resume = null,
                        Count = form.Count,
                        Status = form.Status,
                        Post = form.Post,
                        Applicant = form.Applicant,
                    })
                    .ToListAsync();

                response.limit = limit;
                response.offset = offset;
                response.totalInPage = list.Count();
                response.totalItems = context.ApplicantPosts
                    .Include(form => form.Applicant)
                    .Include(form => form.Post)
                    .Where(form => form.PostId == postId)
                    .Where(form => statuses.Contains(form.Status)).Count();
                response.data = list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " Error at GetAllPosts: " + ex.Message);
            }
            return response;
        }

        public async Task<IEnumerable<ApplicantPost>> GetApplicationFormsByUserIdAndPostIdAsync(Guid postId, Guid userId)
        {
            List<ApplicantPost> list = new List<ApplicantPost>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.ApplicantPosts
                    .Include(form => form.Post)
                    .Include(form => form.Applicant)
                    .Where(form => form.ApplicantId == userId && form.PostId == postId)
                    .Select(form => new ApplicantPost()
                    {
                        PostId = form.PostId,
                        ApplicantId = form.ApplicantId,
                        Message = form.Message,
                        Resume = null,
                        Count = form.Count,
                        Status = form.Status,
                        Post = form.Post,
                        Applicant = form.Applicant,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetApplicationFormsByUserIdAndPostIdAsync: " + ex.Message);
            }
            return list;
        }

        public async Task<DaoResponse<string>> CreateApplicationFormPost(Guid postId, Guid userId, string message, byte[] resume)
        {
            try
            {
                var context = new eRecruitmentContext();
                var post = await PostsDAO.Instance.GetPostByIdAsync(postId);
                if (post == null || post.Status != CommonEnums.POST_STATUS.Available)
                {
                    throw new Exception("Cannot apply this post anymore!");
                }
                var listForms = await GetApplicationFormsByUserIdAndPostIdAsync(postId, userId);
                if (listForms.FirstOrDefault(
                    form  => form.Status != CommonEnums.APPLICATION_FORM_STATUS.Rejected && 
                    form.Status != CommonEnums.APPLICATION_FORM_STATUS.Revoked) != null)
                {
                    throw new Exception("You are having an unprocessed or approved form!");
                }
                var maxCountForms = listForms.OrderByDescending(item => item.Count).FirstOrDefault();
                int currentCount = maxCountForms == null ? 1 : maxCountForms.Count + 1;
                ApplicantPost applicantPost = new ApplicantPost()
                {
                    ApplicantId = userId,
                    PostId = postId,
                    Message = message,
                    Resume = resume,
                    Status = CommonEnums.APPLICATION_FORM_STATUS.Open,
                    Count = currentCount
                };
                context.ApplicantPosts.Add(applicantPost);
                context.Entry(applicantPost).State = EntityState.Added;

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Create Application Form Post successfully!");
                    return new DaoResponse<string>(true, "");
                }
                else
                {
                    throw new Exception("Create application form failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at CreateApplicationFormPost: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }

        }

        public async Task<ApplicantPost> GetOpenStatusFormByPostIdAndUserId(Guid postId, Guid userId)
        {
            ApplicantPost form = null;
            try
            {
                var context = new eRecruitmentContext();
                ApplicantPost applicantPost = context.ApplicantPosts
                    .Include(p => p.Post)
                    .Where(app => app.PostId == postId && app.ApplicantId == userId)
                    .OrderByDescending(item => item.Count)
                    .FirstOrDefault();

                if (applicantPost == null || applicantPost.Post == null || applicantPost.Post.Status != CommonEnums.POST_STATUS.Available || applicantPost.Status != CommonEnums.APPLICATION_FORM_STATUS.Open)
                {
                    throw new Exception("Cannot found open status form of this user!");
                }
                form = applicantPost;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetOpenStatusFormByPostIdAndUserId: " + ex.Message);
            }
            return form;
        }

        public ApplicantPost GetFormByPK(Guid postId, Guid userId, int count)
        {
            ApplicantPost form = null;
            try
            {
                var context = new eRecruitmentContext();
                ApplicantPost applicantPost = context.ApplicantPosts
                    .Include(p => p.Post)
                    .Include(p => p.Applicant)
                    .FirstOrDefault(app => app.PostId == postId && app.ApplicantId == userId && app.Count == count);
                form = applicantPost ?? throw new Exception("Form not found!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetOpenStatusFormByPostIdAndUserId: " + ex.Message);
            }
            return form;
        }

        public DaoResponse<string> UpdateResumeOfPostApplication(Guid postId, Guid userId, byte[] resume)
        {
            try
            {
                var context = new eRecruitmentContext();
                ApplicantPost applicantPost = context.ApplicantPosts
                    .Include(p => p.Post)
                    .Where(app => app.PostId == postId && app.ApplicantId == userId)
                    .OrderByDescending(item => item.Count)
                    .FirstOrDefault();

                if (applicantPost == null || applicantPost.Post == null || applicantPost.Post.Status != CommonEnums.POST_STATUS.Available)
                {
                    throw new Exception("Cannot edit form of this post anymore!");
                }
                applicantPost.Resume = resume;
                context.ApplicantPosts.Update(applicantPost);
                context.Entry(applicantPost).State = EntityState.Modified;

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "UpdateResumeOfPostApplication successfully!");
                    return new DaoResponse<string>(true, "");
                } else
                {
                    throw new Exception("Update application form failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at UpdateResumeOfPostApplication: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }

        }
        public async Task<IEnumerable<ApplicantPost>> GetListAllFormsOfAnUser(Guid userId)
        {
            List<ApplicantPost> list = new List<ApplicantPost>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.ApplicantPosts
                    .Include(form => form.Post)
                    .Include(form => form.Applicant)
                    .Where(form => form.ApplicantId == userId)
                    .Select(form => new ApplicantPost()
                    {
                        PostId = form.PostId,
                        ApplicantId = form.ApplicantId,
                        Message = form.Message,
                        Resume = null,
                        Count = form.Count,
                        Status = form.Status,
                        Post = form.Post,
                        Applicant = form.Applicant
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetListAllFormsOfAnUser: " + ex.Message);
            }
            return list;
        }
        public async Task<DaoResponse<string>> ApproveAnApplicationForm(Guid postId, Guid userId)
        {
            try
            {
                var context = new eRecruitmentContext();
                var form = await GetOpenStatusFormByPostIdAndUserId(postId, userId);

                if (form == null)
                {
                    throw new Exception("Application form not found!!!");
                }
                form.Status = CommonEnums.APPLICATION_FORM_STATUS.Approved;
                var otherPostsForms = await context.ApplicantPosts
                    .Include(form => form.Post)
                    .Include(form => form.Applicant)
                    .Where(form => form.ApplicantId == userId && form.Status == CommonEnums.APPLICATION_FORM_STATUS.Open && form.PostId != postId)
                    .ToListAsync();
                foreach(var otherOpenForm in otherPostsForms)
                {
                    otherOpenForm.Status = CommonEnums.APPLICATION_FORM_STATUS.Pending;
                    context.ApplicantPosts.Update(otherOpenForm);
                    context.Entry(otherOpenForm).State = EntityState.Modified;
                }
                context.ApplicantPosts.Update(form);
                context.Entry(form).State = EntityState.Modified;

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "ApproveAnApplicationForm successfully!");
                    return new DaoResponse<string>(true, "");
                }
                else
                {
                    throw new Exception("Approve an application form failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at ApproveAnApplicationForm: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }

        }

        public async Task<DaoResponse<string>> RejectAnApplicationForm(Guid postId, Guid userId)
        {
            try
            {
                var context = new eRecruitmentContext();
                var form = context.ApplicantPosts
                    .Include(p => p.Post)
                    .Where(app => app.PostId == postId && app.ApplicantId == userId)
                    .OrderByDescending(item => item.Count)
                    .FirstOrDefault();

                if (form == null)
                {
                    throw new Exception("Application form not found!!!");
                }
                if (form.Status != CommonEnums.APPLICATION_FORM_STATUS.Approved && form.Status != CommonEnums.APPLICATION_FORM_STATUS.Open)
                {
                    throw new Exception("Application form not found!!!");
                }

                // approved -> rejected

                if (form.Status == CommonEnums.APPLICATION_FORM_STATUS.Approved)
                {
                    var otherPostsForms = context.ApplicantPosts
                    .Include(form => form.Post)
                    .Include(form => form.Applicant)
                    .Where(form => form.ApplicantId == userId && form.Status == CommonEnums.APPLICATION_FORM_STATUS.Pending && form.PostId != postId)
                    .ToList();
                    foreach (var otherPendingForms in otherPostsForms)
                    {
                        otherPendingForms.Status = CommonEnums.APPLICATION_FORM_STATUS.Open;
                        context.ApplicantPosts.Update(otherPendingForms);
                    }
                }
                form.Status = CommonEnums.APPLICATION_FORM_STATUS.Rejected;
                context.ApplicantPosts.Update(form);

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "RejectAnApplicationForm successfully!");
                    return new DaoResponse<string>(true, "");
                }
                else
                {
                    throw new Exception("Reject an application form failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at RejectAnApplicationForm: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }

        }

        public async Task<DaoResponse<string>> RevokeAnApplicationForm(Guid postId, Guid userId)
        {
            try
            {
                var context = new eRecruitmentContext();
                var form = await GetOpenStatusFormByPostIdAndUserId(postId, userId);

                if (form == null)
                {
                    throw new Exception("Application form not found!!!");
                }
                form.Status = CommonEnums.APPLICATION_FORM_STATUS.Revoked;
                context.ApplicantPosts.Update(form);
                context.Entry(form).State = EntityState.Modified;

                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "RevokeAnApplicationForm successfully!");
                    return new DaoResponse<string>(true, "");
                }
                else
                {
                    throw new Exception("Revoke an application form failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at RevokeAnApplicationForm: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }
        }
    }
}
