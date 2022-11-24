using BusinessObject;
using BusinessObject.DTO;
using DataAccess.DAO;
using DataAccess.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class InterviewRepository : IInterviewRepository
    {
        public async Task<DaoResponse<string>> CreateInterview(Guid interviewerId, DateTime start, DateTime end, Guid postId, Guid applicantId)
            => await InterviewDAO.Instance.CreateInterview(interviewerId, start, end, postId, applicantId);
        public async Task<DaoResponse<string>> DeleteInterviewById(Guid postId, Guid applicantId, int round) => await InterviewDAO.Instance.DeleteInterviewById(postId, applicantId, round);
        public Task<IEnumerable<Interview>> GetAllInterview() => InterviewDAO.Instance.GetAllInterview();
        public Task<IEnumerable<Interview>> GetAllInterviewByPostId(Guid postId) => InterviewDAO.Instance.GetAllInterviewByPostId(postId);
        public Task<IEnumerable<Interview>> GetAllInterviewByApplicantId(Guid applicantId) => InterviewDAO.Instance.GetAllInterviewByApplicantId(applicantId);
        public Task<IEnumerable<Interview>> GetAllInterviewByInterviewerId(Guid interviewerId) => InterviewDAO.Instance.GetAllInterviewByInterviewerId(interviewerId);
        public Task<Interview> GetInterviewByPostIdApplicantIdAndRound(Guid postId, Guid applicantId, int round) => InterviewDAO.Instance.GetInterviewByPostIdApplicantIdAndRound(postId, applicantId, round);
        public async Task<DaoResponse<string>> UpdateInterview(Guid interviewerId, Guid postId, Guid applicantId, int round, DateTime start, DateTime end, string feedback, bool results)
            => await InterviewDAO.Instance.UpdateInterview(interviewerId, postId, applicantId, round, start, end, feedback, results);
        public async Task<IEnumerable<Tuple<ApplicantPost, bool>>> GetApprovedFormWithInterviewStatus()
            => await InterviewDAO.Instance.GetApprovedFormWithInterviewStatus();
        public Task<IEnumerable<Interview>> GetListEditableInterviews() => InterviewDAO.Instance.GetListEditableInterviews();

    }
}
