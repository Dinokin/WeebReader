using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class NovelChapterContent : BaseEntity
    {
        public string Content { get; set; }
        public Guid ChapterId { get; }
        public NovelChapter? Chapter { get; private set; }

        public NovelChapterContent(string content, Guid chapterId) : this(default, content, chapterId) { }

        public NovelChapterContent(Guid id, string content, Guid chapterId) : base(id)
        {
            Content = content;
            ChapterId = chapterId;
        }
    }
}