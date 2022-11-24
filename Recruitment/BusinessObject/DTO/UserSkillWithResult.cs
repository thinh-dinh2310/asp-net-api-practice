using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UserSkillWithResult
    {
        public Guid userId { get; set; }
        public List<Skill> missingSkills { get; set; }

        public UserSkillWithResult()
        {
            missingSkills = new List<Skill>();
        }
    }
}
