using System;
using System.Collections.Generic;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; }
        public IEnumerable<TitleTag>? TitleTag { get; set; }

        public Tag(string name) : this (default, name) { }

        public Tag(Guid id, string name) : base(id) => Name = name;
    }
}