using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class ComicPage : Page
    {
        public ushort Number { get; set; }
        public ComicChapter? Chapter { get; set; }

        public ComicPage(bool animated, Guid chapterId, ushort number) : base(animated, chapterId)
        {
            Number = number;
        }

        public ComicPage(Guid id, bool animated, Guid chapterId, ushort number, ComicChapter? chapter) : base(id, animated, chapterId)
        {
            Number = number;
            Chapter = chapter;
        }
    }
}