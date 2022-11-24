using BusinessObject;
using BusinessObject.DTO;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IFormRepository
    {
        Task<DaoResponse<string>> CreateApplicationFormPost(Guid postId, Guid userId, string message, byte[] resume);
        DaoResponse<string> UpdateResumeOfApplicationPost(Guid postId, Guid userId, byte[] resume);
        Task<PaginationResult<ApplicantPost>> GetAllApplicationFormOfOnePost(Guid postId, int offset, int limit, int status);
        Task<IEnumerable<ApplicantPost>> GetListAllFormsOfAnUser(Guid userId);
        ApplicantPost GetApplicationPostDetailByPK(Guid postId, Guid userId, int count);
        Task<DaoResponse<string>> ApproveAForm(Guid postId, Guid userId);
        Task<DaoResponse<string>> RejectAForm(Guid postId, Guid userId);
        Task<DaoResponse<string>> RevokeAForm(Guid postId, Guid userId);

    }
}
