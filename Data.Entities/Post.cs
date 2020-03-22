using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Visible { get; set; }

        public Post(string title, string content, DateTime releaseDate, bool visible)
        {
            Title = title;
            Content = content;
            ReleaseDate = releaseDate;
            Visible = visible;
        }

        public Post(Guid id, string title, string content, DateTime releaseDate, bool visible) : base(id)
        {
            Title = title;
            Content = content;
            ReleaseDate = releaseDate;
            Visible = visible;
        }
    }
}