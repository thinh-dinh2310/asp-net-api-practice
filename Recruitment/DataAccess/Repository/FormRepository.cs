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
    public class FormRepository : IFormRepository
    {
        public async Task<DaoResponse<string>> CreateApplicationFormPost(Guid postId, Guid userId, string message, byte[] resume)
            => await FormDAO.Instance.CreateApplicationFormPost(postId, userId, message, resume);

        public DaoResponse<string> UpdateResumeOfApplicationPost(Guid postId, Guid userId, byte[] resume)
            => FormDAO.Instance.UpdateResumeOfPostApplication(postId, userId, resume);

        public async Task<PaginationResult<ApplicantPost>> GetAllApplicationFormOfOnePost(Guid postId, int offset, int limit, int status)
            => await FormDAO.Instance.GetAllApplicationFormOfOnePost(postId, offset, limit, status);

        public async Task<IEnumerable<ApplicantPost>> GetListAllFormsOfAnUser(Guid userId)
            => await FormDAO.Instance.GetListAllFormsOfAnUser(userId);

        public ApplicantPost GetApplicationPostDetailByPK(Guid postId, Guid userId, int count)
            => FormDAO.Instance.GetFormByPK(postId, userId, count);

        public async Task<DaoResponse<string>> ApproveAForm(Guid postId, Guid userId)
            => await FormDAO.Instance.ApproveAnApplicationForm(postId, userId);

        public async Task<DaoResponse<string>> RejectAForm(Guid postId, Guid userId)
            => await FormDAO.Instance.RejectAnApplicationForm(postId, userId);
        public async Task<DaoResponse<string>> RevokeAForm(Guid postId, Guid userId)
            => await FormDAO.Instance.RevokeAnApplicationForm(postId, userId);
    }
}
