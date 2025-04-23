using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using utlraReaderNETversion02.Models;
using utlraReaderNETversion02.ViewModels;

public class WebtoonListController : Controller
{
    private readonly IWebHostEnvironment _env;

    public WebtoonListController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public IActionResult Index()
    {
        var webtoonsPath = Path.Combine(_env.WebRootPath, "webtoons");
        var webtoonDirs = Directory.GetDirectories(webtoonsPath);

        var model = webtoonDirs
            .Select(dir => new WebtoonListItemViewModel
            {
                Name = Path.GetFileName(dir),
                Info = LoadWebtoonInfo(dir)
            })
            .Where(x => x.Info != null)
            .ToList();

        return View(model);
    }

    private WebtoonInfo LoadWebtoonInfo(string directory)
    {
        var infoPath = Path.Combine(directory, "info.json");
        if (!System.IO.File.Exists(infoPath))
            return null;

        var json = System.IO.File.ReadAllText(infoPath);
        return System.Text.Json.JsonSerializer.Deserialize<WebtoonInfo>(json);
    }

    public IActionResult Details(string name)
    {
        if (string.IsNullOrEmpty(name)) return NotFound();

        string infoPath = Path.Combine(_env.WebRootPath, "webtoons", name, "info.json");

        if (!System.IO.File.Exists(infoPath))
            return NotFound();

        var json = System.IO.File.ReadAllText(infoPath);
        var info = JsonConvert.DeserializeObject<WebtoonInfo>(json);

        ViewData["WebtoonName"] = name;
        return View(info);
    }
}
