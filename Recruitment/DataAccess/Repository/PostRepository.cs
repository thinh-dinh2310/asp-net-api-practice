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
    public class PostRepository : IPostRepository
    {
        public Task<PaginationResult<PostViewModel>> GetAllPosts(int offset, int limit, string locations, string category, string skills, string levels, string keywords)
            => PostsDAO.Instance.GetAllPosts(offset, limit, locations, category, skills, levels, keywords);
        public Task<PostViewModel> GetPostVMByIdAsync(Guid PostId) => PostsDAO.Instance.GetPostVMByIdAsync(PostId);
        public Task<Post> GetPostByIdAsync(Guid PostId) => PostsDAO.Instance.GetPostByIdAsync(PostId);
        public async Task UpdatePostById(Guid postId, Guid categoryId, string title, string description, Guid[] postLocationsIds, Guid[] postSkillsIds)
            => await PostsDAO.Instance.UpdatePostById(postId, categoryId,title, description,postLocationsIds,postSkillsIds);
        public async Task<DaoResponse<string>> UpdateStatusPost(Guid PostId, int status) => await PostsDAO.Instance.UpdatePostStatus(PostId, status);
        public void CreatePost(Guid categoryId, string title, string description, Guid[] postLocationsIds, Guid[] postSkillsIds, Guid levelId)
            => PostsDAO.Instance.CreatePost(categoryId, title, description, postLocationsIds, postSkillsIds, levelId);
        public Tuple<IEnumerable<Category>, IEnumerable<Skill>, IEnumerable<Location>, IEnumerable<Level>> InitDataForCreationOrUpdationPostPage()
        {
            return PostsDAO.Instance.InitDataForCreationOrUpdationPostPage();
        }
    }
}
