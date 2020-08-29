﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class Comic : Title
    {
        public bool LongStrip { get; set; }
        public IEnumerable<ComicChapter>? Chapters { get; private set; }

        public Comic(string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, bool longStrip, string? previousChaptersLink = null) :
            this(default, name, originalName, author, artist, synopsis, status, nsfw, visible, longStrip, previousChaptersLink) { }

        public Comic(Guid id, string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, bool longStrip, string? previousChaptersLink = null) :
            base(id, name, originalName, author, artist, synopsis, status, nsfw, visible, previousChaptersLink)
        {
            LongStrip = longStrip;
        }
    }
}