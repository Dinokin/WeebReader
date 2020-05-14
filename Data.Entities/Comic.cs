using System;
using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Comic : Title
    {
        public bool LongStrip { get; set; }
        public IEnumerable<ComicChapter>? Chapters { get; set; }

        public Comic(string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool visible, bool longStrip, string? previousChaptersLink = null)
            : base(name, originalName, author, artist, synopsis, status, visible, previousChaptersLink)
        {
            LongStrip = longStrip;
        }

        public Comic(Guid id, string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool visible, bool longStrip, string? previousChaptersLink = null)
            : base(id, name, originalName, author, artist, synopsis, status, visible, previousChaptersLink)
        {
            LongStrip = longStrip;
        }
    }
}