using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Novel : Title
    {
        public IEnumerable<NovelChapter> Chapters { get; set; }
    }
}