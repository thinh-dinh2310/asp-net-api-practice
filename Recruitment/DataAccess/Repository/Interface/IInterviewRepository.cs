using BusinessObject;
using BusinessObject.DTO;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.Interface
{
    public interface IInterviewRepository
    {
        Task<IEnumerable<Interview>> GetAllInterview();
        Task<Interview> GetInterviewByPostIdApplicantIdAndRound(Guid postId, Guid applicantId, int round);
        Task<IEnumerable<Interview>> GetAllInterviewByPostId(Guid postId);
        Task<IEnumerable<Interview>> GetAllInterviewByApplicantId(Guid applicantId);
        Task<IEnumerable<Interview>> GetAllInterviewByInterviewerId(Guid interviewerId);
        Task<DaoResponse<string>> CreateInterview(Guid interviewerId, DateTime start, DateTime end, Guid postId, Guid applicantId);
        Task<DaoResponse<string>> UpdateInterview(Guid interviewerId, Guid postId, Guid applicantId, int round, DateTime start, DateTime end, string feedback, bool results);
        Task<DaoResponse<string>> DeleteInterviewById(Guid postId, Guid applicantId, int round);
        Task<IEnumerable<Tuple<ApplicantPost, bool>>> GetApprovedFormWithInterviewStatus();
        Task<IEnumerable<Interview>> GetListEditableInterviews();
    }
}
