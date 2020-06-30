using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class NovelPage : Page
    {
        public NovelChapter? Chapter { get; private set; }

        public NovelPage(bool animated, Guid chapterId) : this(default, animated, chapterId) { }

        public NovelPage(Guid id, bool animated, Guid chapterId) : base(id, animated, chapterId) { }
    }
}