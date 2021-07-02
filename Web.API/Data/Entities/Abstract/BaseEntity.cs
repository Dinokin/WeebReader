using System;

namespace WeebReader.Web.API.Data.Entities.Abstract
{
    public abstract record BaseEntity
    {
        public Guid Id { get; init; }
    }
}