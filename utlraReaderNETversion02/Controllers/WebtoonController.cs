using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using utlraReaderNETversion02.Models;

namespace utlraReaderNETversion02.Controllers
{
    public class WebtoonController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public WebtoonController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Detay Sayfası
        public IActionResult Details(string name)
        {
            // name parametresi boşsa hata döndür
            if (string.IsNullOrEmpty(name))
                return BadRequest("Webtoon adı geçersiz.");

            // Webtoon bilgilerini yükle
            var webtoonInfo = LoadWebtoonInfo(name);
            if (webtoonInfo == null)
                return NotFound("Webtoon bulunamadı.");

            // Bölümlerin fiziksel yolunu kontrol et
            string chaptersPath = Path.Combine(_env.WebRootPath, "webtoons", name);

            // Debug için kritik log (Output Penceresinde görünür)
            System.Diagnostics.Debug.WriteLine($"Bölüm yolu: {chaptersPath}");
            System.Diagnostics.Debug.WriteLine($"Klasör var mı? {Directory.Exists(chaptersPath)}");

            // Bölüm klasörlerini al
            var chapterFolders = new List<string>();
            if (Directory.Exists(chaptersPath))
            {
                chapterFolders = Directory.GetDirectories(chaptersPath)
                    .Select(Path.GetFileName)
                    .Where(folder =>
                        !string.IsNullOrEmpty(folder) &&
                        !folder.Equals("info.json", StringComparison.OrdinalIgnoreCase) &&
                        !folder.Equals(webtoonInfo.Cover, StringComparison.OrdinalIgnoreCase)) // Cover dosyasını hariç tut
                    .OrderBy(x => x, new NumericAndTextComparer())
                    .ToList();
            }

            ViewData["WebtoonName"] = name;
            ViewData["Chapters"] = chapterFolders;

            return View(webtoonInfo);
        }

        private List<string> GetChapterFolders(string chaptersPath)
        {
            if (!Directory.Exists(chaptersPath))
                return new List<string>();

            return Directory.GetDirectories(chaptersPath)
                .Select(Path.GetFileName)
                .Where(folder => !string.IsNullOrEmpty(folder) && !folder.Equals("info.json", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, new NumericAndTextComparer())
                .ToList();
        }

        // Bölüm Resimlerini Yükleme
        public IActionResult Read(string webtoonName, string chapterName)
        {
            string chapterPath = Path.Combine(_env.WebRootPath, "webtoons", webtoonName, chapterName);
            if (!Directory.Exists(chapterPath))
                return NotFound("Bölüm bulunamadı.");

            var images = Directory.GetFiles(chapterPath, "*.jpg")
                .Select(Path.GetFileName)
                .OrderBy(x => x, new NumericAndTextComparer())
                .ToList();

            ViewData["WebtoonName"] = webtoonName;
            ViewData["ChapterName"] = chapterName;
            ViewData["Images"] = images;

            return View();
        }

        // Sayısal ve Metinsel Sıralama için Karşılaştırıcı
        private class NumericAndTextComparer : IComparer<string>
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

        // Webtoon bilgilerini yükleyen yardımcı metod
        private WebtoonInfo LoadWebtoonInfo(string name)
        {
            string jsonPath = Path.Combine(_env.WebRootPath, "webtoons", name, "info.json");
            if (!System.IO.File.Exists(jsonPath))
                return null;

            string json = System.IO.File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<WebtoonInfo>(json);
        }




        // Düzenleme sayfası (GET)
        public IActionResult Edit(string name)
        {
            var webtoonInfo = LoadWebtoonInfo(name);
            if (webtoonInfo == null)
                return NotFound();

            ViewData["WebtoonName"] = name;
            return View(webtoonInfo);
        }

        // Düzenleme sonrası kaydetme (POST)
        [HttpPost]
        public IActionResult Edit(string name, WebtoonInfo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string jsonFile = GetJsonPath(name);

            try
            {
                var json = JsonConvert.SerializeObject(model, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFile, json);
            }
            catch (Exception ex)
            {
                // hata durumunda kullanıcıya mesaj göstermek istenirse
                ModelState.AddModelError("", "Kaydetme sırasında hata oluştu: " + ex.Message);
                return View(model);
            }

            return RedirectToAction("Details", new { name = name });
        }

        // Dosya yolunu oluşturan yardımcı metod
        private string GetJsonPath(string name)
        {
            // wwwroot'u doğrudan WebRootPath üzerinden al
            return Path.Combine(_env.WebRootPath, "webtoons", name, "info.json");
        }
    }
}
