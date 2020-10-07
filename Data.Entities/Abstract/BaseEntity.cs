using System;
using System.Diagnostics.CodeAnalysis;

 namespace WeebReader.Data.Entities.Abstract
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }

        protected BaseEntity() : this(default) { }
        
        protected BaseEntity(Guid id) => Id = id;
    }
}