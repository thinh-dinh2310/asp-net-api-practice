using BusinessObject.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class FeedbackDTO
    {
        public Guid InterviewerId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Feedback { get; set; }
        public bool Result { get; set; }
        public int Round { get; set; }
        public Guid PostId { get; set; }
        public Guid ApplicantId { get; set; }
        public FeedbackDTO()
        {

        }

        public FeedbackDTO(Interview interview)
        {
            this.InterviewerId = interview.InterviewerId;
            this.StartDateTime = interview.StartDateTime;
            this.EndDateTime = interview.EndDateTime;
            this.Feedback = interview.Feedback;
            this.Result = interview.Result;
            this.Round = interview.Round;
            this.PostId = interview.PostId;
            this.ApplicantId = interview.ApplicantId;
        }
    }
    public class InterviewVM : Interview
    {
        public bool CanEdit { get; set; }

        public InterviewVM()
        {

        }

        public InterviewVM(Interview interview, bool canEdit)
        {
            InterviewerId = interview.InterviewerId;
            StartDateTime = interview.StartDateTime;
            EndDateTime = interview.EndDateTime;
            Feedback = interview.Feedback;
            Result = interview.Result;
            Round = interview.Round;
            PostId = interview.PostId;
            ApplicantId = interview.ApplicantId;
            CanEdit = canEdit;
        }
    }
}
