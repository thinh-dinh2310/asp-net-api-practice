using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class APWithMissingSkill
    {
        public List<UserSkillWithResult> userMissingSkills { get; set; }
        public PaginationResult<ApplicantPost> ap { get; set; }
        public APWithMissingSkill()
        {

        }
    }
}
