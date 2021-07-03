using System;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.Entities
{
    public class NovelChapterContent : BaseEntity
    {
        public Guid ChapterId { get; init; }
        public string Content { get; set; } = string.Empty;
        
        public NovelChapter? Chapter { get; init; }
    }
}