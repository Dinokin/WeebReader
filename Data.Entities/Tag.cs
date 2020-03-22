using System;
using System.Collections.Generic;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public IEnumerable<TitleTag> TitleTag { get; } = new List<TitleTag>();

        public Tag(string name)
        {
            Name = name;
        }

        public Tag(Guid id, string name) : base(id)
        {
            Name = name;
        }
    }
}