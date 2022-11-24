using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class Category
    {
        public Category()
        {
            Posts = new HashSet<Post>();
        }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
