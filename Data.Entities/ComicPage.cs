using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class ComicPage : Page
    {
        public ushort Number { get; }
        public ComicChapter? Chapter { get; set; }

        public ComicPage(bool animated, Guid chapterId, ushort number) : this(default, animated, chapterId, number) { }

        public ComicPage(Guid id, bool animated, Guid chapterId, ushort number) : base(id, animated, chapterId) => Number = number;
    }
}