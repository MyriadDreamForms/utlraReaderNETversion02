namespace utlraReaderNETversion02.Models.ViewModels
{
    public class ChapterImageViewModel
    {
        // Mevcut dosya adı
        public string FileName { get; set; }
        // Düzenlendikten sonra dosya adı; varsayılan olarak mevcut ad
        public string NewName { get; set; }
        // Silinmek istenip istenmediğini belirtmek için
        public bool Delete { get; set; }
    }
}
