using BusinessObject;
using BusinessObject.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UsersDAO
    {
        private static UsersDAO instance = null;
        private static readonly object instanceLock = new object();
        private UsersDAO() { }

        public static UsersDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new UsersDAO();
                    }
                }
                return instance;
            }
        }

        public async Task<List<Skill>> GetOneUsersCompareSkill(Guid postId, Guid userId)
        {
            List<Skill> missingSkill = new List<Skill>();
            try
            {
                var context = new eRecruitmentContext();
                var postSkill = await context.PostSkills.Include(post => post.Skills)
                    .Where(post => post.PostId == postId).ToListAsync();

                var userSkill = await context.UserSkills.Include(user => user.Skills)
                    .Where(post => post.UsersId == userId).ToListAsync();

                foreach(var item in postSkill)
                {
                    bool check = false;
                    foreach(var userItem in userSkill)
                    {
                        if(item.SkillsId == userItem.SkillsId)
                        {
                            check = true;
                        }
                    }

                    if (!check)
                    {
                        var missSkill = await context.Skills.FirstOrDefaultAsync(sk => sk.SkillsId == item.SkillsId);
                        missingSkill.Add(missSkill);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllUsers: " + ex.Message);
            }
            return missingSkill;
        }

        public async Task<List<UserSkillWithResult>> GetUsersCompareSkill(Guid postId)
        {
            List<UserSkillWithResult> listUser = new List<UserSkillWithResult>();
            List<User> list = new List<User>();
            try
            {
                var context = new eRecruitmentContext();
                var postSkill = await context.PostSkills.Include(post => post.Skills)
                    .Where(post => post.PostId == postId).ToListAsync();

                var applicantPosts = await context.ApplicantPosts
                    .Where(post => post.PostId == postId)
                    .ToListAsync();

                foreach(var item in applicantPosts)
                {
                    UserSkillWithResult us = new UserSkillWithResult();
                    us.userId = item.ApplicantId;
                    us.missingSkills = new List<Skill>();
                    
                    var userSkill = await context.UserSkills.Include(user => user.Skills)
                        .Where(user => user.UsersId == item.ApplicantId).ToListAsync();

                    foreach (var ps in postSkill)
                    {
                        bool check = false;
                        foreach (var userItem in userSkill)
                        {
                            if (ps.SkillsId == userItem.SkillsId)
                            {
                                check = true;
                            }
                        }

                        if (!check)
                        {
                            var missSkill = await context.Skills.FirstOrDefaultAsync(sk => sk.SkillsId == ps.SkillsId);
                            us.missingSkills.Add(missSkill);
                        }
                    }

                    listUser.Add(us);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllUsers: " + ex.Message);
            }
            return listUser;
        }

        public async Task<PaginationResult<User>> GetAllUsers(int offset = 0, int limit = 10, string keywords = "")
        {
            PaginationResult<User> response = new PaginationResult<User>();
            List<User> list = new List<User>();
            try
            {
                keywords = String.IsNullOrEmpty(keywords) ? "" : keywords.Trim();
                var context = new eRecruitmentContext();
                list = await context.Users
                    .Include(u => u.Role)
                    .Where(k => k.Email.ToLower().Contains(keywords))
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip(offset * limit)
                    .Take(limit)
                    .ToListAsync();

                response.limit = limit;
                response.offset = offset;
                response.totalInPage = list.Count();
                response.totalItems = context.Users.Count(k => k.Email.ToLower().Contains(keywords));
                response.data = list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllUsers: " + ex.Message);
            }
            return response;
        }

        public async Task<IEnumerable<User>> GetAllUsersByRoleId(Guid roleId)
        {
            List<User> list = new List<User>();
            try
            {
                var context = new eRecruitmentContext();
                list = await context.Users
                                .Where(acc => acc.RoleId == roleId)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllUsersByRoles: " + ex.Message);
            }
            return list;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            User tmp = null;
            try
            {
                var context = new eRecruitmentContext();
                tmp = await context.Users
                                    .Include(u => u.UserSkills)
                                    .ThenInclude(u => u.Skills)
                                    .FirstOrDefaultAsync(m => m.Id == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetUserByIdAsync: " + ex.Message);
            }
            return tmp;
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            User tmp = null;
            try
            {
                var context = new eRecruitmentContext();
                tmp = await context.Users
                                    .Include(u => u.Role)
                                    .FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower()
                                                                && m.Password == password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetUserByEmailAndPasswordAsync: " + ex.Message);
            }
            return tmp;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            User tmp = null;
            try
            {
                var context = new eRecruitmentContext();
                tmp = await context.Users
                                    .Include(u => u.Role)
                                    .FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetUserByEmail: " + ex.Message);
            }
            return tmp;
        }

        public void UpdateUserById(Guid userId, string email, string password, string firstName, string lastName, string displayName, string phone, DateTime dateOfBirth,
            string address, string description, byte[] resume, byte[] avatar)
        {
            try
            {
                var context = new eRecruitmentContext();
                User user = context.Users.FirstOrDefault(u => u.Id == userId);
                user.Email = email.Trim();
                user.Password = password.Trim();
                user.FirstName = firstName.Trim();
                user.LastName = lastName.Trim();
                user.DisplayName = displayName.Trim();
                user.Phone = phone.Trim();
                user.DateOfBirth = dateOfBirth;
                user.Address = address.Trim();
                user.Description = description.Trim();
                user.Resume = resume;
                user.Avatar = avatar;
                user.UpdatedAt = DateTime.Now;
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at UpdateUserById: " + ex.Message);
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.Now;
                var db = new eRecruitmentContext();
                if (user.Resume == null)
                {
                    User tmp = db.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Id == user.Id).Result;
                    user.Resume = tmp.Resume;
                }
                db.Users.Update(user);
                foreach (var item in user.UserSkills)
                {
                    UserSkill us = db.UserSkills.AsNoTracking().FirstOrDefaultAsync(p => p.SkillsId == item.SkillsId && p.UsersId == item.UsersId).Result;
                    if (us == null)
                    {
                        db.UserSkills.Add(item);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void DeleteUserById(Guid userId)
        {
            try
            {
                var context = new eRecruitmentContext();
                User user = context.Users.FirstOrDefault(u => u.Id.Equals(userId));
                context.Users.Remove(user);
                //    user.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at DeleteUserById: " + ex.Message);
            }
        }

        public void DeleteUserSkill(UserSkill userSkill)
        {
            try
            {
                var context = new eRecruitmentContext();
                context.UserSkills.Remove(userSkill);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at DeleteUserById: " + ex.Message);
            }
        }

        public void CreateUser(Guid userRoleId, string email, string password, string firstName, string lastName, string phone, string address)
        {
            try
            {
                //string role = "";
                //if (userRole.Equals("USER", StringComparison.OrdinalIgnoreCase))
                //{
                //    role = USER_ROLE;
                //}
                //if (userRole.Equals("HR", StringComparison.OrdinalIgnoreCase))
                //{
                //    role = HR_ROLE;
                //}
                //if (userRole.Equals("INTERVIEWER", StringComparison.OrdinalIgnoreCase))
                //{
                //    role = INTERVIEWER_ROLE;
                //}

                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    RoleId = userRoleId, //resident
                    Email = email.Trim(),
                    Password = password.Trim(),
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    Phone = phone.Trim(),
                    Address = address.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };
                var context = new eRecruitmentContext();
                context.Users.Add(user);
                if (context.SaveChanges() > 0)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Created user successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at CreateUser: " + ex.Message);
            }
        }

        public void CreateUser(User user)
        {
            var db = new eRecruitmentContext();
            User users = null;
            users = db.Users.Where(p => p.Email.Contains(user.Email)
                           || p.Id.ToString().Contains(user.Id.ToString())).SingleOrDefault();

            if (users != null)
            {
                throw new Exception("User has already exist");
            }
            db.Users.Add(user);
            db.SaveChanges();
        }

        public void InsertRefreshToken(string refreshToken, Guid userId)
        {
            try
            {
                var context = new eRecruitmentContext();
                List<AuthToken> listCurrentUserToken = context.AuthTokens.Where(token => token.UserId == userId).ToList();
                foreach (AuthToken token in listCurrentUserToken)
                {
                    context.AuthTokens.Remove(token);
                }
                AuthToken authToken = new AuthToken()
                {
                    AuthTokenId = Guid.NewGuid(),
                    RefreshToken = refreshToken,
                    UserId = userId,
                };
                context.AuthTokens.Add(authToken);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at InsertRefreshToken: " + ex.Message);
            }
        }

        public AuthToken GetAuthTokenByRefreshToken(string refreshToken)
        {
            AuthToken tmp = new AuthToken();
            try
            {
                var context = new eRecruitmentContext();
                tmp = context.AuthTokens.FirstOrDefault(token => token.RefreshToken == refreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at InsertRefreshToken: " + ex.Message);
            }
            return tmp;
        }

        public void DeleteAuthToken(string refreshToken)
        {
            try
            {
                var context = new eRecruitmentContext();
                AuthToken authToken = context.AuthTokens.FirstOrDefault(token => token.RefreshToken == refreshToken);
                context.AuthTokens.Remove(authToken);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at InsertRefreshToken: " + ex.Message);
            }
        }


    }
}
