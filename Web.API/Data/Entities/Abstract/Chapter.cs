using System;

namespace WeebReader.Web.API.Data.Entities.Abstract
{
    public abstract class Chapter : BaseEntity
    {
        public ushort? Volume { get; set; }
        public decimal Number { get; set; }
        public string? Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Visible { get; set; }
        public Guid TitleId { get; init; }
    }
}