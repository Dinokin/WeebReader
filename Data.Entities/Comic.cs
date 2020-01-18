using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Comic : Title
    {
        public bool LongStrip { get; set; }
        
        public IEnumerable<ComicChapter> Chapters { get; set; }
    }
}