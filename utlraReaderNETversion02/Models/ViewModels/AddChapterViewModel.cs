using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace utlraReaderNETversion02.Models.ViewModels
{
    public class AddChapterViewModel
    {
        [Required]
        public string WebtoonName { get; set; }

        [Required]
        public string ChapterName { get; set; }

        [Required]
        public List<IFormFile> Images { get; set; }
    }
}
