using System;
using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Novel : Title
    {
        public IEnumerable<NovelChapter>? Chapters { get; set; }

        public Novel(string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, string? previousChaptersLink = null)
            : base(name, originalName, author, artist, synopsis, status, nsfw, visible, previousChaptersLink) { }

        public Novel(Guid id, string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, string? previousChaptersLink = null) 
            : base(id, name, originalName, author, artist, synopsis, status, nsfw, visible, previousChaptersLink) { }
    }
}