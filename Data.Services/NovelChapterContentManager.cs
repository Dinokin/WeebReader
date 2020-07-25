using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class NovelChapterContentManager : GenericManager<NovelChapterContent>
    {
        public NovelChapterContentManager(BaseContext context) : base(context) { }
        
        public async Task<NovelChapterContent> GetContentByChapter(NovelChapter novelChapter) => await DbSet.SingleOrDefaultAsync(content => content.ChapterId == novelChapter.Id);
    }
}