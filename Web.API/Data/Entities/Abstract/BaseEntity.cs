using System;

namespace WeebReader.Web.API.Data.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; }
    }
}