using System;

namespace WeebReader.Web.API.Data.Entities.Abstract
{
    public abstract class Page : BaseEntity
    {
        public bool Animated { get; init; }
        public Guid ChapterId { get; init; }
    }
}