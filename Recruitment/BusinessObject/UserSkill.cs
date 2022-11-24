using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class UserSkill
    {
        public Guid SkillsId { get; set; }
        public Guid UsersId { get; set; }

        public virtual Skill Skills { get; set; }
        public virtual User Users { get; set; }
    }
}
