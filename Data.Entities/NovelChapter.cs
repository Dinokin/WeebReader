using System;
using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class NovelChapter : Chapter
    {
        public string Content { get; set; }
        public Novel? Title { get; private set; }
        public IEnumerable<NovelPage>? Pages { get; private set; }

        public NovelChapter(ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId, string content) : this(default, volume, number, name, releaseDate, visible, titleId, content) { }

        public NovelChapter(Guid id, ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId, string content) : base(id, volume, number, name, releaseDate, visible, titleId)
        {
            Content = content;
        }
    }
}