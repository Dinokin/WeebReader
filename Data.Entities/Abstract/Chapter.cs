using System;

namespace WeebReader.Data.Entities.Abstract
{
    public abstract class Chapter : BaseEntity
    {
        public ushort? Volume { get; set; }
        public decimal Number { get; set; }
        public string? Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Visible { get; set; }
        public Guid TitleId { get; set; }

        protected Chapter(ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId)
        {
            Volume = volume;
            Number = number / 1.0000000000000000000000000000m;
            Name = name;
            ReleaseDate = releaseDate;
            Visible = visible;
            TitleId = titleId;
        }

        protected Chapter(Guid id, ushort? volume, decimal number, string? name, DateTime releaseDate, bool visible, Guid titleId) : base(id)
        {
            Volume = volume;
            Number = number / 1.0000000000000000000000000000m;
            Name = name;
            ReleaseDate = releaseDate;
            Visible = visible;
            TitleId = titleId;
        }
    }
}