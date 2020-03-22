using System;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Page : BaseEntity
    {
        public bool Animated { get; set; }
        public Guid ChapterId { get; set; }

        protected Page(bool animated, Guid chapterId)
        {
            Animated = animated;
            ChapterId = chapterId;
        }

        protected Page(Guid id, bool animated, Guid chapterId) : base(id)
        {
            Animated = animated;
            ChapterId = chapterId;
        }
    }
}