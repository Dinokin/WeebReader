﻿using System;

 namespace WeebReader.Data.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }

        protected BaseEntity() { }
        
        protected BaseEntity(Guid id) => Id = id;
    }
}