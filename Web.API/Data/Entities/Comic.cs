using System.Collections.Generic;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class Comic : Title
    {
        public bool LongStrip { get; set; }
        
        public IEnumerable<ComicChapter>? Chapters { get; set; }
    }
}