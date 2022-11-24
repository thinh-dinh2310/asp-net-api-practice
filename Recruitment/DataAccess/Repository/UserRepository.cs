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
    public class UserRepository : IUserRepository
    {
        public async Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            return await UsersDAO.Instance.GetUserByEmailAndPasswordAsync(email, password);
        }

        public async Task<List<UserSkillWithResult>> GetUserComparedSkill(Guid postId)
        {
            return await UsersDAO.Instance.GetUsersCompareSkill(postId);
        }
        public async Task<List<Skill>> GetOneUsersCompareSkill(Guid postId, Guid userId)
        {
            return await UsersDAO.Instance.GetOneUsersCompareSkill(postId, userId);
        }
        public async Task<User> GetUserById(Guid userId)
        {
            return await UsersDAO.Instance.GetUserByIdAsync(userId);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await UsersDAO.Instance.GetUserByEmailAsync(email);
        }


        public void RegisterUser(User user)
        {
            UsersDAO.Instance.CreateUser(user.RoleId, user.Email, user.Password, user.FirstName, user.LastName, user.Phone, user.Address);
        }

        public void InsertRefreshToken(string refreshToken, Guid userId)
        {
            UsersDAO.Instance.InsertRefreshToken(refreshToken, userId);
        }

        public AuthToken GetRefreshToken(string refreshToken)
        {
            return UsersDAO.Instance.GetAuthTokenByRefreshToken(refreshToken);
        }
        public void DeleteAuthToken(string refreshToken)
        {
            UsersDAO.Instance.DeleteAuthToken(refreshToken);
        }
        public Task<PaginationResult<User>> GetAllUsers(int offset, int limit, string keywords) => UsersDAO.Instance.GetAllUsers(offset, limit, keywords);
        public Task<IEnumerable<User>> GetAllUsersByRoleId(Guid roleId) => UsersDAO.Instance.GetAllUsersByRoleId(roleId);
        public Task<User> GetUserByIdAsync(Guid userId) => UsersDAO.Instance.GetUserByIdAsync(userId);
        public void UpdateUserById(Guid userId, string email, string password, string firstName, string lastName, string displayName, string phone, DateTime dateOfBirth,
            string address, string description, byte[] resume, byte[] avatar)
            => UsersDAO.Instance.UpdateUserById(userId, email, password, firstName, lastName, displayName, phone, dateOfBirth, address, description, resume, avatar);
        public void DeleteUserById(Guid userId) => UsersDAO.Instance.DeleteUserById(userId);
        public void CreateUser(string userRole, string email, string password, string firstName, string lastName, string phone, string address)
            => UsersDAO.Instance.CreateUser(Guid.Parse(userRole), email, password, firstName, lastName, phone, address);

        public void CreateUser(User user) => UsersDAO.Instance.CreateUser(user);
        public void UpdateUser(User user)
        
            => UsersDAO.Instance.UpdateUser(user);
        public void DeleteUserSkill(UserSkill userSkill) => UsersDAO.Instance.DeleteUserSkill(userSkill);
    }
}
