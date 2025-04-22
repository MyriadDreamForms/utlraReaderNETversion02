using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using utlraReaderNETversion02.Models;
using utlraReaderNETversion02.Models.ViewModels;

namespace utlraReaderNETversion02.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public IActionResult Index(string webtoon, string chapter)

        {
            string chapterPath = Path.Combine(_env.WebRootPath, "webtoons", webtoon, chapter);
            if (!Directory.Exists(chapterPath))
                return NotFound("Bölüm bulunamadı.");

            var images = Directory.GetFiles(chapterPath, "*.jpg")
                .Select(Path.GetFileName)
                .OrderBy(x => x, new NumericAndTextComparer())
                .ToList();

            ViewData["WebtoonName"] = webtoon;
            ViewData["ChapterName"] = chapter;
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


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name, string displayName, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(displayName))
            {
                ViewBag.Error = "Zorunlu alanlar eksik.";
                return View();
            }

            // Yol: wwwroot/webtoons/{name}
            var webRoot = _env.WebRootPath;
            var webtoonPath = Path.Combine(webRoot, "webtoons", name);

            if (Directory.Exists(webtoonPath))
            {
                ViewBag.Error = "Bu isimde bir webtoon zaten var.";
                return View();
            }

            Directory.CreateDirectory(webtoonPath);

            var info = new WebtoonInfo
            {
                Title = displayName,
                Description = description ?? "",
                Cover = null
            };

            System.IO.File.WriteAllText(
                Path.Combine(webtoonPath, "info.json"),
                JsonConvert.SerializeObject(info, Formatting.Indented)
            );

            return RedirectToAction("List");
        }


        [HttpGet]
        public IActionResult AddChapter(string name)
        {
            return View(new AddChapterViewModel { WebtoonName = name });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddChapter(AddChapterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string chapterPath = Path.Combine(_env.WebRootPath, "webtoons", model.WebtoonName, model.ChapterName);

            if (Directory.Exists(chapterPath))
            {
                ModelState.AddModelError("", "Bu isimde bir bölüm zaten var.");
                return View(model);
            }

            Directory.CreateDirectory(chapterPath);

            int index = 1;
            foreach (var file in model.Images)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{index:D3}{extension}";
                    var filePath = Path.Combine(chapterPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    index++;
                }
            }

            TempData["Message"] = "Bölüm başarıyla eklendi.";
            return RedirectToAction("Details", new { name = model.WebtoonName });
        }


        [HttpPost]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Webtoon adı geçersiz.");

            string webtoonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "webtoons", name);

            if (!Directory.Exists(webtoonPath))
                return NotFound("Webtoon bulunamadı.");

            try
            {
                Directory.Delete(webtoonPath, true); // true: içeriğiyle birlikte sil
                TempData["Message"] = $"{name} başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Silme işlemi başarısız: " + ex.Message;
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult DeleteChapter(string webtoon, string chapter)
        {
            if (string.IsNullOrWhiteSpace(webtoon) || string.IsNullOrWhiteSpace(chapter))
                return BadRequest("Geçersiz parametre.");

            string chapterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "webtoons", webtoon, chapter);

            if (!Directory.Exists(chapterPath))
                return NotFound("Bölüm bulunamadı.");

            try
            {
                Directory.Delete(chapterPath, true); // klasör ve içeriği silinir
                TempData["Message"] = $"'{chapter}' bölümü silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Silme hatası: " + ex.Message;
            }

            return RedirectToAction("Details", new { name = webtoon });
        }






    }
}
