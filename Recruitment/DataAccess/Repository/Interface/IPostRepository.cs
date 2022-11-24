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
    public interface IPostRepository
    {
        Task<PaginationResult<PostViewModel>> GetAllPosts(int offset, int limit, string locations, string category, string skills, string levels, string keywords);
        Task<PostViewModel> GetPostVMByIdAsync(Guid PostId);
        Task<Post> GetPostByIdAsync(Guid PostId);

        Task UpdatePostById(Guid postId, Guid categoryId, string title, string description, Guid[] postLocationsIds, Guid[] postSkillsIds);
        Task<DaoResponse<string>> UpdateStatusPost(Guid PostId, int status);
        void CreatePost(Guid categoryId, string title, string description, Guid[] postLocationsIds, Guid[] postSkillsIds, Guid levelId);
        Tuple<IEnumerable<Category>, IEnumerable<Skill>, IEnumerable<Location>, IEnumerable<Level>> InitDataForCreationOrUpdationPostPage();
    }
}
