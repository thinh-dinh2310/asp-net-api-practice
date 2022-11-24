using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class SkillRepository : ISkillRepository
    {
        public Task<IEnumerable<Skill>> GetAllSkill() => SkillDAO.Instance.GetAllSkills();
    }
}
