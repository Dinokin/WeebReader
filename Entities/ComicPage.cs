using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class ComicPage : Page
    {
        public ushort Number { get; set; }
        
        public ComicChapter Chapter { get; set; }
    }
}