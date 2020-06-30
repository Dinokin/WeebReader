using System;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Page : BaseEntity
    {
        public bool Animated { get; }
        public Guid ChapterId { get; }

        protected Page(bool animated, Guid chapterId) : this(default, animated, chapterId) { }

        protected Page(Guid id, bool animated, Guid chapterId) : base(id)
        {
            Animated = animated;
            ChapterId = chapterId;
        }
    }
}