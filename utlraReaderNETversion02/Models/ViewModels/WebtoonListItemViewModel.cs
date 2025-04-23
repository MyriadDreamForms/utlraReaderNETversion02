namespace utlraReaderNETversion02.ViewModels
{
    public class WebtoonListItemViewModel
    {
        public string Name { get; set; }              // Klasör ismi (örnek: "solo-leveling")
        public WebtoonInfo Info { get; set; }         // info.json'dan gelen bilgiler
    }
}
