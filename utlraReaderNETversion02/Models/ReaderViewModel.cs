public class ReaderViewModel
{
    public List<string> Images { get; set; }
    public string WebtoonName { get; set; }
    public string ChapterName { get; set; }
    public string PreviousChapter { get; set; } // string olarak değiştirildi
    public string NextChapter { get; set; }     // Chapter class'ına gerek yok
}