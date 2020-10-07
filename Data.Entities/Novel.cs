using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class Novel : Title
    {
        public IEnumerable<NovelChapter>? Chapters { get; private set; }

        public Novel(string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, string? previousChaptersUrl = null)
            : this(default, name, originalName, author, artist, synopsis, status, nsfw, visible, previousChaptersUrl) { }

        public Novel(Guid id, string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, string? previousChaptersUrl = null)
            : base(id, name, originalName, author, artist, synopsis, status, nsfw, visible, previousChaptersUrl) { }
    }
}