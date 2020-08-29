using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class Tag : BaseEntity
    {
        public string Name { get; }
        public IEnumerable<TitleTag>? TitleTag { get; private set; }

        public Tag(string name) : this (default, name) { }

        public Tag(Guid id, string name) : base(id)
        {
            Name = name;
        }
    }
}