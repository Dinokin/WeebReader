using System.Collections.Generic;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{ 
    public class NovelChapter : Chapter
    {
        public Novel? Title { get; set; }
        public NovelChapterContent? Content { get; set; }
        public IEnumerable<NovelPage>? Pages { get; set; }
    }
}