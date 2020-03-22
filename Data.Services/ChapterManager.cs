using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class ChapterManager<TChapter> : GenericManager<TChapter> where TChapter : Chapter
    {
        public ChapterManager(BaseContext context) : base(context) { }

        public async Task<long> CountChaptersByTitle(Title title) => await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id);

        public Task<IEnumerable<TChapter>> GetChaptersByTitle(Title title, int skip, int take) => Task.FromResult<IEnumerable<TChapter>>(DbSet.Where(chapter => chapter.TitleId == title.Id)
            .OrderByDescending(chapter => chapter.Number).Skip(skip).Take(take));
    }
}