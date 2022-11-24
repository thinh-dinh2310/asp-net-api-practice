using BusinessObject;
using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DataAccess.DAO
{
    public class InterviewDAO
    {
        private static InterviewDAO instance = null;
        private static readonly object instanceLock = new object();

        private InterviewDAO() { }

        public static InterviewDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InterviewDAO();
                    }
                }
                return instance;
            }
        }

        public async Task<IEnumerable<Interview>> GetAllInterview()
        {
            List<Interview> list = new List<Interview>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.Interviews
                    .Include(p => p.Applicant)
                    .Include(p => p.Interviewer)
                    .Include(p => p.Post)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllInterviews: " + ex.Message);
            }
            return list;
        }

        public async Task<Interview> GetInterviewByPostIdApplicantIdAndRound(Guid postId, Guid applicantId, int round)
        {
            Interview tmp = new Interview();
            try
            {
                var context = new eRecruitmentContext();
                tmp = await context.Interviews
                    .Include(p => p.Applicant)
                    .Include(p => p.Interviewer)
                    .Include(p => p.Post)
                    .FirstOrDefaultAsync(i => i.PostId == postId && i.ApplicantId == applicantId && i.Round == round);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetInterviewByPostIdApplicantIdAndRound: " + ex.Message);
            }
            return tmp;
        }

        public async Task<IEnumerable<Interview>> GetAllInterviewByPostId(Guid postId)
        {
            List<Interview> list = new List<Interview>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.Interviews
                    .Include(p => p.Applicant)
                    .Include(p => p.Interviewer)
                    .Include(p => p.Post)
                    .Where(p => p.PostId == postId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllInterviewByPostId: " + ex.Message);
            }
            return list;
        }

        public async Task<IEnumerable<Interview>> GetAllInterviewByApplicantId(Guid applicantId)
        {
            List<Interview> list = new List<Interview>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.Interviews
                    .Include(p => p.Applicant)
                    .Include(p => p.Interviewer)
                    .Include(p => p.Post)
                    .Where(p => p.ApplicantId == applicantId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllInterviewByApplicantId: " + ex.Message);
            }
            return list;
        }

        public async Task<IEnumerable<Interview>> GetAllInterviewByInterviewerId(Guid interviewerId)
        {
            List<Interview> list = new List<Interview>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.Interviews
                    .Include(p => p.Applicant)
                    .Include(p => p.Interviewer)
                    .Include(p => p.Post)
                    .Where(p => p.InterviewerId == interviewerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllInterviewByInterviewẻId: " + ex.Message);
            }
            return list;
        }

        public async Task<DaoResponse<string>> CreateInterview(Guid interviewerId, DateTime start, DateTime end, Guid postId, Guid applicantId)
        {
            try
            {
                var context = new eRecruitmentContext();
                var latestInterview = (await GetAllInterviewByApplicantId(applicantId)).OrderByDescending(i => i.Round).FirstOrDefault();
                if (latestInterview != null && latestInterview.Result == false)
                {
                    throw new Exception("The previous interview of this candidate is failed or not feedbacked yet!");
                }
                var occupiedInterviewerTime = context.Interviews.FirstOrDefault(i => i.InterviewerId == interviewerId &&
                ((start >= i.StartDateTime && start <= i.EndDateTime) || (end >= i.StartDateTime && end <= i.EndDateTime)) ||
                (start < i.StartDateTime && end > i.EndDateTime));
                if (occupiedInterviewerTime != null)
                {
                    throw new Exception("This interviewer is busy from "+ occupiedInterviewerTime.StartDateTime.ToString("yyyy/MM/dd/ HH:mm:ss") + " to " + occupiedInterviewerTime.EndDateTime.ToString("yyyy/MM/dd/ HH:mm:ss"));
                }
                Interview newInterview = new Interview()
                {
                    InterviewerId = interviewerId,
                    StartDateTime = start,
                    EndDateTime = end,
                    Feedback = "",
                    Result = false,
                    Round = (latestInterview == null ? 0 : latestInterview.Round) + 1,
                    PostId = postId,
                    ApplicantId = applicantId
                };
                context.Interviews.Add(newInterview);
                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Created Interview successfully");
                    return new DaoResponse<string>(true, "");
                }
                else
                {
                    throw new Exception("Create interview failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at CreateInterview: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }
        }

        public async Task<DaoResponse<string>> UpdateInterview(Guid interviewerId, Guid postId, Guid applicantId, int round, DateTime start, DateTime end, string feedback, bool result)
        {
            try
            {
                var context = new eRecruitmentContext();
                Interview tmp = await context.Interviews
                    .FirstOrDefaultAsync(i => i.PostId == postId && i.ApplicantId == applicantId && i.Round == round);
                /*tmp = await context.Interviews
                    .FirstOrDefaultAsync(p => p.IsDeleted != true && p.InterviewId == interviewId);*/
                //tmp.InterviewerId = interviewerId;
                tmp.StartDateTime = start;
                tmp.EndDateTime = end;
                tmp.Feedback = feedback;
                tmp.Result = result;
                var occupiedInterviewerTime = context.Interviews.FirstOrDefault(i => i.InterviewerId == tmp.InterviewerId && !(tmp.ApplicantId == i.ApplicantId && tmp.InterviewerId == i.InterviewerId && tmp.Round == i.Round && tmp.PostId == i.PostId)  &&
                ((start >= i.StartDateTime && start <= i.EndDateTime) || (end >= i.StartDateTime && end <= i.EndDateTime)) ||
                (start < i.StartDateTime && end > i.EndDateTime));
                if (occupiedInterviewerTime != null)
                {
                    throw new Exception("This interviewer is busy from " + occupiedInterviewerTime.StartDateTime.ToString("yyyy/MM/dd/ HH:mm:ss") + " to " + occupiedInterviewerTime.EndDateTime.ToString("yyyy/MM/dd/ HH:mm:ss"));
                }
                //context.Entry(tmp).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return new DaoResponse<string>(true, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at UpdateInterview: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }
        }

        public async Task<DaoResponse<string>> DeleteInterviewById(Guid postId, Guid applicantId, int round)
        {
            try
            {
                var context = new eRecruitmentContext();
                Interview interview = await context.Interviews.FirstOrDefaultAsync(i => i.PostId == postId && i.ApplicantId == applicantId && i.Round == round);
                if(interview == null)
                {
                    throw new Exception("Interview not found!");
                }
                if (interview.Result == true || interview.Feedback != "")
                {
                    throw new Exception("Cannot delete feedbacked interview!");
                }
                if (interview.StartDateTime <= DateTime.Now)
                {
                    throw new Exception("Cannot delete started interview!");
                }
                context.Interviews.Remove(interview);
                await context.SaveChangesAsync();
                return new DaoResponse<string>(true, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at DeleteInterviewById: " + ex.Message);
                return new DaoResponse<string>(false, ex.Message);
            }
        }

        public async Task<IEnumerable<Tuple<ApplicantPost, bool>>> GetApprovedFormWithInterviewStatus()
        {
            List<Tuple<ApplicantPost, bool>> res = new List<Tuple<ApplicantPost, bool>>();
            try
            {
                var context = new eRecruitmentContext();
                List<ApplicantPost> list = await context.ApplicantPosts
                    .Include(form => form.Post)
                    .Include(form => form.Applicant)
                    .Where(form => form.Status == CommonEnums.APPLICATION_FORM_STATUS.Approved)
                    .ToListAsync();
                foreach(var ele in list)
                {
                    var interviewing = context.Interviews
                        .Where(i => i.PostId == ele.PostId && i.ApplicantId == ele.ApplicantId)
                        .OrderByDescending(i => i.Round)
                        .FirstOrDefault();

                    if (ele.Post.Status != CommonEnums.POST_STATUS.Pending || (interviewing != null && interviewing.Result == false))
                    {
                        res.Add(new Tuple<ApplicantPost, bool>(ele, false));
                        continue;
                    }
                    res.Add(new Tuple<ApplicantPost, bool>(ele, true));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetApprovedFormWithInterviewStatus: " + ex.Message);
            }
            return res;

        }
        public async Task<IEnumerable<Interview>> GetListEditableInterviews()
        {
            List<Interview> list = new List<Interview>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.Interviews
                    .Include(p => p.Applicant)
                    .Include(p => p.Interviewer)
                    .Include(p => p.Post)
                    .Where(p => p.Post.Status == CommonEnums.POST_STATUS.Pending && p.EndDateTime < DateTime.Now && p.Round == context.Interviews
                                                    .Where(p2 => p2.ApplicantId == p.ApplicantId 
                                                    && p2.InterviewerId == p.InterviewerId && p2.PostId == p.PostId).Select(row => row.Round).Max())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllInterviews: " + ex.Message);
            }
            return list;
        }

    }
}
