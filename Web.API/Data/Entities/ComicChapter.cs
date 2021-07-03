using System.Collections.Generic;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class ComicChapter : Chapter
    {
        public Comic? Title { get; init; }
        public IEnumerable<ComicPage>? Pages { get; set; }
    }
}