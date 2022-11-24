using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class User
    {
        public User()
        {
            ApplicantPosts = new HashSet<ApplicantPost>();
            AuthTokens = new HashSet<AuthToken>();
            InterviewApplicants = new HashSet<Interview>();
            InterviewInterviewers = new HashSet<Interview>();
            UserSkills = new HashSet<UserSkill>();
        }

        
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage="Invalid email address")]
         public string Email { get; set; }

        public string Password { get; set; }

        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "Only accepted characters from A to Z")]
        [MaxLength(20, ErrorMessage = "The length must be in range from 2 to 20 characters")]
        [MinLength(2, ErrorMessage = "The length must be in range from 2 to 20 characters")]
        public string FirstName { get; set; }


        [RegularExpression(@"[a-zA-Z]*", ErrorMessage = "Only accepted characters from A to Z")]
        public string LastName { get; set; }
       
        public string DisplayName { get; set; }

        [RegularExpression(@"[0-9]*", ErrorMessage = "Invalid phone numbers")]
        [MaxLength(11, ErrorMessage = "Invalid phone numbers")]
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20, ErrorMessage = "The length must be in range from 2 to 20 characters")]
        [MinLength(2, ErrorMessage = "The length must be in range from 2 to 20 characters")]
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Resume { get; set; }
        public byte[] Avatar { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<ApplicantPost> ApplicantPosts { get; set; }
        public virtual ICollection<AuthToken> AuthTokens { get; set; }
        public virtual ICollection<Interview> InterviewApplicants { get; set; }
        public virtual ICollection<Interview> InterviewInterviewers { get; set; }
        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }
}
