using Microsoft.AspNetCore.Mvc;
using utlraReaderNETversion02.Models;
using Newtonsoft.Json;
public class WebtoonListController : Controller
{
    public IActionResult Index()
    {
        var webtoons = LoadWebtoons();
        return View(webtoons);
    }

    private List<Webtoon> LoadWebtoons()
    {
        string jsonFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "webtoons.json");
        var json = System.IO.File.ReadAllText(jsonFile);
        return JsonConvert.DeserializeObject<List<Webtoon>>(json);
    }
}
