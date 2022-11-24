using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class Level
    {
        public Level()
        {
            Posts = new HashSet<Post>();
        }

        public Guid LevelId { get; set; }
        public string LevelName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
