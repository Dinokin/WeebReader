using System.Collections.Generic;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class Novel : Title
    {
        public IEnumerable<NovelChapter>? Chapters { get; set; }
    }
}