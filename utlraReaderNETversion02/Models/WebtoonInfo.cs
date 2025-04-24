public class WebtoonInfo
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string Cover { get; set; }

    public string Genre { get; set; }

    public string Status { get; set; } // Örn: Devam Ediyor, Tamamlandı
    public List<Chapter> Chapters { get; set; }


    //public List<string> Chapters { get; set; } = new List<string>();

}

public class Chapter
{
    public int Number { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
}
