using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class LocationPost
    {
        public Guid LocationId { get; set; }
        public Guid PostId { get; set; }

        public virtual Location Location { get; set; }
        public virtual Post Post { get; set; }
    }
}
