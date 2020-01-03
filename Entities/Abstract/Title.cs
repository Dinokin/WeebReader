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
        public string OriginalName { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Synopsis { get; set; }
        public byte[] Cover { get; set; }
        public Statuses Status { get; set; }
        public bool Visible { get; set; }

        public IEnumerable<TitleTag> TitleTags { get; set; }
    }
}