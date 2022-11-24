using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class SkillDAO
    {
        private static SkillDAO instance = null;
        private static readonly object instanceLock = new object();
        private SkillDAO() { }

        public static SkillDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SkillDAO();
                    }
                }
                return instance;
            }
        }

        public async Task<IEnumerable<Skill>> GetAllSkills()
        {
            List<Skill> list = new List<Skill>();
            try
            {
                var context = new eRecruitmentContext();
                list = context.Skills.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at GetAllSkill: " + ex.Message);
            }
            return list;
        }
    }
}
