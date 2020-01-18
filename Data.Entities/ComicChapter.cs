using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class ComicChapter : Chapter
    {
        public Comic Title { get; set; }
        public IEnumerable<ComicPage> Pages { get; set; }
    }
}