using System;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class NovelPage : Page
    {
        public NovelChapter? Chapter { get; set; }

        public NovelPage(bool animated, Guid chapterId) : base(animated, chapterId) { }

        public NovelPage(Guid id, bool animated, Guid chapterId) : base(id, animated, chapterId) { }
    }
}