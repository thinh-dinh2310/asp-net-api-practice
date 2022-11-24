using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class PostSkill
    {
        public Guid PostId { get; set; }
        public Guid SkillsId { get; set; }

        public virtual Post Post { get; set; }
        public virtual Skill Skills { get; set; }
    }
}
