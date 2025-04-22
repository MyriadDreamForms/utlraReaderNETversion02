using Newtonsoft.Json;

namespace utlraReaderNETversion02.Models
{
    public class Webtoon
    {
        public string Name { get; set; }              // Dosya ve klasör adı
        public string DisplayName { get; set; }       // Kullanıcıya görünen 
       public string Cover { get; set; }             // Kapak görseli dosya adı
        public string Description { get; set; }       // Açıklama yazısı

    }
}
