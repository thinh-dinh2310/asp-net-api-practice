using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class MissingSkillWithPVM
    {
        public List<Skill> missingSkill { get; set; }
        public PostViewModel pvm { get; set; }
        public MissingSkillWithPVM()
        {

        }
    }
}
