using System;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class NovelPage : Page
    {
        public NovelChapter? Chapter { get; private set; }

        public NovelPage(bool animated, Guid chapterId) : base(animated, chapterId) { }

        public NovelPage(Guid id, bool animated, Guid chapterId) : base(id, animated, chapterId) { }
    }
}