using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace BusinessObject
{
    public partial class Skill
    {
        public Skill()
        {
            PostSkills = new HashSet<PostSkill>();
            UserSkills = new HashSet<UserSkill>();
        }
        [Key]
        public Guid SkillsId { get; set; }
        public string SkillName { get; set; }

        public virtual ICollection<PostSkill> PostSkills { get; set; }
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
