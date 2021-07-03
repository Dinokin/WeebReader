using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class NovelPage : Page
    {
        public NovelChapter? Chapter { get; init; }
    }
}