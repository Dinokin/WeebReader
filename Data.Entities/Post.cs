using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Post : BaseEntity
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
    }
}