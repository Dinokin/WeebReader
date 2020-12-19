using System;

 namespace WeebReader.Data.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; }

        protected BaseEntity() : this(default) { }
        
        protected BaseEntity(Guid id) => Id = id;
    }
}