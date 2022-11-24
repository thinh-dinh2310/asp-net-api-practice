using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable disable

namespace BusinessObject
{
    public partial class ApplicantPost
    {
        [Key]
        public Guid ApplicantId { get; set; }
        [Key]
        public Guid PostId { get; set; }
        public byte[] Resume { get; set; }
        public string Message { get; set; }
        [Key]
        public int Count { get; set; }
        public int Status { get; set; }

        public virtual User Applicant { get; set; }
        public virtual Post Post { get; set; }
    }
}
