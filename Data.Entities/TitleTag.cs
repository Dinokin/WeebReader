using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class TitleTag : BaseEntity
    {
        public Guid TitleId { get; }
        public Guid TagId { get; }
        public Title? Title { get; private set; }
        public Tag? Tag { get; private set; }

        public TitleTag(Guid titleId, Guid tagId) : this(default, titleId, tagId) { }

        public TitleTag(Guid id, Guid titleId, Guid tagId) : base(id)
        {
            TitleId = titleId;
            TagId = tagId;
        }
    }
}