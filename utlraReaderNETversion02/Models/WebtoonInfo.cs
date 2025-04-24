public class WebtoonInfo
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string Cover { get; set; }
    public List<ChapterInfo> Chapters { get; set; } = new();
}

public class ChapterInfo
{
    public int Number { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
}
