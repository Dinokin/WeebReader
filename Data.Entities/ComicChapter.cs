using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class ComicChapter : Chapter
    {
        public Comic? Title { get; private set; }
        public IEnumerable<ComicPage>? Pages { get; private set; }

        public ComicChapter(ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId)
            : this(default, volume, number, name, releaseDate, visible, titleId) { }

        public ComicChapter(Guid id, ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId)
            : base(id, volume, number, name, releaseDate, visible, titleId) { }
    }
}