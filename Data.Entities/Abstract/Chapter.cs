using System;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Chapter : BaseEntity
    {
        public ushort Volume { get; set; }
        public decimal Number { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool Visible { get; set; }
        public Guid TitleId { get; set; }
        public byte Type { get; set; }
    }
}