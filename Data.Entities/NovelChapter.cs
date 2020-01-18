using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class NovelChapter : Chapter
    {
        public string Content { get; set; }
        
        public Novel Title { get; set; }
    }
}