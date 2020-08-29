using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class NovelChapter : Chapter
    {
        public Novel? Title { get; private set; }
        public NovelChapterContent? NovelChapterContent { get; set; }
        public IEnumerable<NovelPage>? Pages { get; private set; }

        public NovelChapter(ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId) :
            base(volume, number, name, releaseDate, visible, titleId) { }

        public NovelChapter(Guid id, ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId) :
            base(id, volume, number, name, releaseDate, visible, titleId) { }
    }
}