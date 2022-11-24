using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class Location
    {
        public Location()
        {
            LocationPosts = new HashSet<LocationPost>();
        }

        public Guid LocationId { get; set; }
        public string LocationName { get; set; }

        public virtual ICollection<LocationPost> LocationPosts { get; set; }
    }
}
