using System;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class TitleTag : BaseEntity
    {
        public Guid TitleId { get; }
        public Guid TagId { get; }
        public Title? Title { get; set; }
        public Tag? Tag { get; set; }

        public TitleTag(Guid titleId, Guid tagId) : this(default, titleId, tagId) { }

        public TitleTag(Guid id, Guid titleId, Guid tagId) : base(id)
        {
            TitleId = titleId;
            TagId = tagId;
        }
    }
}