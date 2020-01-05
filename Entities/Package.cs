using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Package : Resource
    {
        public Guid ChapterId { get; set; }
        
        public Chapter Chapter { get; set; }
    }
}