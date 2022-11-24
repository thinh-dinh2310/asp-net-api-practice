using BusinessObject;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class PostDto
    {
        public Guid PostId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid LevelId { get; set; }
        public int Status { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ApplicantPost> ApplicantPosts { get; set; }
        public virtual ICollection<PostSkill> PostSkills { get; set; }
        public virtual ICollection<LocationPost> LocationPosts { get; set; }

    }
    public class PostForManipulationDto
    {
        [Required(ErrorMessage = "Post's category is required")]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Level is required")]
        [Display(Name = "Level")]
        public Guid LevelId { get; set; }
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Post's title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Post's description is required")]
        public string Description { get; set; }
        [Display(Name = "Skills")]
        [Required(ErrorMessage = "Skills is required")]
        public Guid[] PostSkillsIds { get; set; }
        [Display(Name = "Location")]
        [Required(ErrorMessage = "Location is required")]
        public Guid[] PostLocationsIds { get; set; }
        

        public PostForManipulationDto(Post post)
        {
            CategoryId = post.CategoryId;
            LevelId = post.LevelId;
            Title = post.Title;
            Description = post.Description;
            PostSkillsIds = post.PostSkills.Select(e => e.SkillsId).ToArray();
            PostLocationsIds = post.LocationPosts.Select(e => e.LocationId).ToArray();
        }
        public PostForManipulationDto() 
        {
        }

    }
    public class PostForUpdationDto : PostForManipulationDto
    {
        public PostForUpdationDto(Post post) : base(post)
        {
            PostId = post.PostId;
        }

        public Guid PostId { get; set; }
        public PostForUpdationDto()
        {
        }
        public PostForUpdationDto(PostViewModel post)
        {
            PostId = post.PostId;
            CategoryId = post.CategoryId;
            LevelId = post.LevelId;
            Title = post.Title;
            Description = post.Description;
            PostSkillsIds = post.PostSkillsIds;
            PostLocationsIds = post.PostLocationsIds;
        }

    }
    public class PostForCreationDto : PostForManipulationDto
    {
        public PostForCreationDto(Post post) : base(post)
        {
        }
        public PostForCreationDto()
        {
        }
    }

    public class PostViewModel : PostForManipulationDto
    {
        public Guid PostId { get; set; }
        public string DisplayingLocations { get; set; }
        public List<string> DisplayingSkills { get; set; }
        public string DisplayingCategory{ get; set; }
        public string DisplayingLevel { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public PostViewModel(Post post) : base(post)
        {
            PostId = post.PostId;
            Status = post.Status;
            DisplayingLocations = string.Join(", ", post.LocationPosts.Select(e => e.Location.LocationName));
            DisplayingSkills = post.PostSkills.Select(e => e.Skills.SkillName).ToList();
            DisplayingCategory = post.Category.CategoryName;
            DisplayingLevel = post.Level.LevelName;
            CreatedAt = post.CreatedAt;
        }
        public PostViewModel()
        {
        }
    }
}
