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

        public async Task<long> Count(Title title, bool includeHidden = false)
        {
            if (includeHidden)
                return await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id);
            
            return await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id && chapter.Visible);
        }
        
        public async Task<IEnumerable<TChapter>> GetRange(int skip, int take, bool includeHidden = false)
        {
            if (includeHidden)
                return await base.GetRange(skip, take);

            return DbSet.Where(chapter => chapter.Visible).Skip(skip).Take(take);
        }
        
        public Task<IEnumerable<TChapter>> GetRange(Title title, int skip, int take, bool includeHidden = false)
        {
            if (includeHidden)
                return Task.FromResult<IEnumerable<TChapter>>(DbSet.Where(chapter => chapter.TitleId == title.Id)
                    .OrderByDescending(chapter => chapter.Number).Skip(skip).Take(take));
            
            return Task.FromResult<IEnumerable<TChapter>>(DbSet.Where(chapter => chapter.TitleId == title.Id && chapter.Visible)
                .OrderByDescending(chapter => chapter.Number).Skip(skip).Take(take));
        }
        
        public Task<IEnumerable<TChapter>> GetAll(Title title, bool includeHidden = false)
        {
            if (includeHidden)
                return Task.FromResult<IEnumerable<TChapter>>(DbSet.Where(chapter => chapter.TitleId == title.Id)
                    .OrderByDescending(chapter => chapter.Number));
            
            return Task.FromResult<IEnumerable<TChapter>>(DbSet.Where(chapter => chapter.TitleId == title.Id && chapter.Visible)
                .OrderByDescending(chapter => chapter.Number));
        }
    }
}