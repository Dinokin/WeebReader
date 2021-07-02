using System;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Visible { get; set; }

        public Post(string title, string content, DateTime releaseDate, bool visible) : this(default, title, content, releaseDate, visible) { }

        public Post(Guid id, string title, string content, DateTime releaseDate, bool visible) : base(id)
        {
            Title = title;
            Content = content;
            ReleaseDate = releaseDate;
            Visible = visible;
        }
    }
}