using BusinessObject.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Interview
    {
        public Guid InterviewerId { get; set; }
        [FutureDateTime]
        public DateTime StartDateTime { get; set; }
        [CompareDatesValidator("StartDateTime")]
        public DateTime EndDateTime { get; set; }
        public string Feedback { get; set; }
        public bool Result { get; set; }
        [Key]
        public int Round { get; set; }
        public Guid PostId { get; set; }
        public Guid ApplicantId { get; set; }

        public virtual User Applicant { get; set; }
        public virtual User Interviewer { get; set; }
        public virtual Post Post { get; set; }
    }
}
