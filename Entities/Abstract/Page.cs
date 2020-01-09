using System;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Page : BaseEntity
    {
        public bool Animated { get; set; }
        public Guid ChapterId { get; set; }
    }
}