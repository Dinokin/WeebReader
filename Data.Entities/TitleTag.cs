using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class TitleTag : BaseEntity
    {
        public Guid TitleId { get; set; }
        public Guid TagId { get; set; }
        
        public Title Title { get; set; }
        public Tag Tag { get; set; }
    }
}