using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class ChaptersManager<TChapter> : GenericManager<TChapter> where TChapter : Chapter
    {
        public ChaptersManager(BaseContext context) : base(context) { }

        public async Task<long> CountChaptersByTitle(Title title) => await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id);

        public Task<IEnumerable<TChapter>> GetChaptersByTitle(Title title, int skip, int take) => Task.FromResult<IEnumerable<TChapter>>(DbSet.Where(chapter => chapter.TitleId == title.Id).Skip(skip).Take(take));
    }
}