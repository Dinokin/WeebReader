using System;
using System.Collections.Generic;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Title : BaseEntity
    {
        public enum Statuses
        {
            Ongoing,
            Hiatus,
            Completed,
            Dropped
        }
        
        public string Name { get; set; }
        public string? OriginalName { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string? Synopsis { get; set; }
        public Statuses Status { get; set; }
        public bool Nsfw { get; set; }    
        public bool Visible { get; set; }
        public string? PreviousChaptersUrl { get; set; }
        public IEnumerable<TitleTag>? TitleTags { get; set; }
        
        protected Title(string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, string? previousChaptersUrl = null)
            : this(default, name, originalName, author, artist, synopsis, status, nsfw, visible, previousChaptersUrl) { }

        protected Title(Guid id, string name, string? originalName, string author, string artist, string? synopsis, Statuses status, bool nsfw, bool visible, string? previousChaptersUrl = null) : base(id)
        {
            Name = name;
            OriginalName = originalName;
            Author = author;
            Artist = artist;
            Synopsis = synopsis;
            Status = status;
            Nsfw = nsfw;
            Visible = visible;
            PreviousChaptersUrl = previousChaptersUrl;
        }
    }
}