using BusinessObject.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ApplicationPostDTO
    {
        
    }

    public class ApplicationPostForCreationDto
    {

        [Required(ErrorMessage = "Something went wrong! Cannot load job information!")]
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "Please leave a resume to tell us more about yourself!")]
        [DataType(DataType.Upload)]
        [MaxFileSize(4 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile Resume { get; set; }

        public string Message { get; set; }

        public ApplicationPostForCreationDto()
        {

        }
    }
}
