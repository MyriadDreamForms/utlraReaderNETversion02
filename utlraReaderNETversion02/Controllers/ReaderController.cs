using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using utlraReaderNETversion02.Models;

namespace utlraReaderNETversion02.Controllers
{
    public class ReaderController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ReaderController(IWebHostEnvironment env)
        {
            _env = env; // IWebHostEnvironment enjekte edildi
        }

        public IActionResult Index(string webtoon, string chapter)
        {
            // Webtoon ve chapter parametrelerini kontrol et
            if (string.IsNullOrEmpty(webtoon) || string.IsNullOrEmpty(chapter))
                return BadRequest("Geçersiz parametre.");

            // Bölüm resimlerini yükle
            var images = LoadChapterImages(webtoon, chapter);
            if (images == null || images.Count == 0)
                return NotFound("Bölüm bulunamadı.");

            // Bölüm listesini otomatik algıla (JSON'dan DEĞİL)
            var chapterList = LoadChapterList(webtoon);
            if (chapterList.Count == 0)
                return NotFound("Bölüm listesi boş.");

            // Geçerli bölümün indeksini bul
            int currentIndex = chapterList.FindIndex(c => c == chapter);
            if (currentIndex == -1)
                return NotFound("Bölüm geçersiz.");

            // ViewModel'i oluştur
            var viewModel = new ReaderViewModel
            {
                Images = images,
                WebtoonName = webtoon,
                ChapterName = chapter,
                PreviousChapter = (currentIndex > 0) ? chapterList[currentIndex - 1] : null,
                NextChapter = (currentIndex < chapterList.Count - 1) ? chapterList[currentIndex + 1] : null
            };

            return View(viewModel);
        }

        private List<string> LoadChapterImages(string webtoon, string chapter)
        {
            // Doğru yol için _env.WebRootPath kullan
            string chapterPath = Path.Combine(_env.WebRootPath, "webtoons", webtoon, chapter);
            if (!Directory.Exists(chapterPath))
                return new List<string>();

            // NumericAndTextComparer ile sırala
            return Directory.GetFiles(chapterPath, "*.jpg")
                .Select(Path.GetFileName)
                .OrderBy(x => x, new NumericAndTextComparer())
                .ToList();
        }

        private List<string> LoadChapterList(string webtoon)
        {
            // Bölümleri JSON'dan DEĞIL, klasör yapısından oku
            string chaptersPath = Path.Combine(_env.WebRootPath, "webtoons", webtoon);
            if (!Directory.Exists(chaptersPath))
                return new List<string>();

            return Directory.GetDirectories(chaptersPath)
                .Select(Path.GetFileName)
                .Where(folder => !string.IsNullOrEmpty(folder) && !folder.Equals("info.json", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, new NumericAndTextComparer())
                .ToList();
        }
    }

    // NumericAndTextComparer sınıfı (WebtoonController'dan taşındı)
    public class NumericAndTextComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (int.TryParse(x, out int numX) && int.TryParse(y, out int numY))
                return numX.CompareTo(numY);

            if (int.TryParse(x, out _)) return -1;
            if (int.TryParse(y, out _)) return 1;

            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}