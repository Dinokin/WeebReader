using System;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class Parameter : BaseEntity
    {
        public ushort Type { get; }
        public string? Value { get; set; }

        public Parameter(ushort type, string? value) : this (default, type, value) { }

        public Parameter(Guid id, ushort type, string? value) : base(id)
        {
            Type = type;
            Value = value;
        }
    }
}