using System;
using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class NovelChapter : Chapter
    {
        public string Content { get; set; }
        public Novel? Title { get; set; }
        public IEnumerable<NovelPage>? Pages { get; set; }

        public NovelChapter(ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId, string content) : base(volume, number, name, releaseDate, visible, titleId)
        {
            Content = content;
        }

        public NovelChapter(Guid id, ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId, string content) : base(id, volume, number, name, releaseDate, visible, titleId)
        {
            Content = content;
        }
    }
}