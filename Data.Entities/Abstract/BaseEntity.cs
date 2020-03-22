﻿using System;

 namespace WeebReader.Data.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private protected set; }

        public BaseEntity() { }
        protected BaseEntity(Guid id) => Id = id;
    }
}