using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace utlraReaderNETversion02.Models.ViewModels
{
    public class EditChapterViewModel
    {
        [Required]
        public string WebtoonName { get; set; }
        [Required]
        public string ChapterName { get; set; }

        // Eğer admin bölüm adını değiştirmek isterse
        public string NewChapterName { get; set; }

        // Mevcut görsellerin düzenlenmesi için
        public List<ChapterImageViewModel> ExistingImages { get; set; } = new List<ChapterImageViewModel>();

        // Yeni eklenmek istenen görseller
        public List<IFormFile> NewImages { get; set; }
    }
}
