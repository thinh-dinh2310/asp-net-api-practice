using System;
using System.Collections.Generic;

#nullable disable

namespace BusinessObject
{
    public partial class AuthToken
    {
        public Guid AuthTokenId { get; set; }
        public string RefreshToken { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
