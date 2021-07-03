using System;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class TitleTag : BaseEntity
    {
        public Guid TitleId { get; init; }
        public Guid TagId { get; init; }
        
        public Title? Title { get; init; }
        public Tag? Tag { get; init; }
    }
}