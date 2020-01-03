using System;
using System.Collections.Generic;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Chapter : BaseEntity
    {
        public ushort Volume { get; set; }
        public decimal Number { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid TitleId { get; set; }
        
        public Package Package { get; set; }
    }
}