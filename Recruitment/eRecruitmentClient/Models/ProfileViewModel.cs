using BusinessObject;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eRecruitmentClient.Models
{
    public class ProfileViewModel
    {
        public User userView { get; set; }
        public IFormFile ResumeFileUpload { get; set; }
        public Guid[] UserSkillsIds { get; set; }

        public ProfileViewModel()
        {

        }
    }
}
