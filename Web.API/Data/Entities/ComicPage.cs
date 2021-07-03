using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class ComicPage : Page
    {
        public ushort Number { get; init; }
        
        public ComicChapter? Chapter { get; init; }
    }
}