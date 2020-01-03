using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Package : BaseEntity
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public Guid ChapterId { get; set; }
        
        public Chapter Chapter { get; set; }
    }
}