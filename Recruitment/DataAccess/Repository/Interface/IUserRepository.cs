using BusinessObject;
using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IUserRepository
    {
        Task<List<UserSkillWithResult>> GetUserComparedSkill(Guid postId);
        Task<List<Skill>> GetOneUsersCompareSkill(Guid postId, Guid userId);
        Task<User> GetUserByEmailAndPassword(string email, string password);
        Task<User> GetUserById(Guid userId);
        Task<User> GetUserByEmail(string email);

        void RegisterUser(User user);
        public void InsertRefreshToken(string refreshToken, Guid userId);
        public AuthToken GetRefreshToken(string refreshToken);
        public void DeleteAuthToken(string refreshToken);

        Task<PaginationResult<User>> GetAllUsers(int offset, int limit, string keywords);
        Task<IEnumerable<User>> GetAllUsersByRoleId(Guid roleId);
        Task<User> GetUserByIdAsync(Guid userId);
        void UpdateUserById(Guid userId, string email, string password, string firstName, string lastName, string displayName, string phone, DateTime dateOfBirth,
            string address, string description, byte[] resume, byte[] avatar);
        void DeleteUserById(Guid userId);
        void CreateUser(string userRole, string email, string password, string firstName, string lastName, string phone, string address);
        void CreateUser(User user);
        void UpdateUser(User usesr);
        void DeleteUserSkill(UserSkill userSkill);
    }
}
