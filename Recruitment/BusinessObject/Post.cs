using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class Post
    {
        public Post()
        {
            ApplicantPosts = new HashSet<ApplicantPost>();
            Interviews = new HashSet<Interview>();
            LocationPosts = new HashSet<LocationPost>();
            PostSkills = new HashSet<PostSkill>();
        }

        public Guid PostId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid LocationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid LevelId { get; set; }
        public int Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<ApplicantPost> ApplicantPosts { get; set; }
        public virtual ICollection<Interview> Interviews { get; set; }
        public virtual ICollection<LocationPost> LocationPosts { get; set; }
        public virtual ICollection<PostSkill> PostSkills { get; set; }
    }
}
