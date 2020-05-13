using System;
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
        
        public async Task<long> Count(bool includeHidden) => includeHidden
            ? await base.Count()
            : await DbSet.LongCountAsync(chapter => chapter.Visible && chapter.ReleaseDate <= DateTime.Now);

        public async Task<long> Count(Title title, bool includeHidden = false) => includeHidden
            ? await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id)
            : await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id && chapter.Visible && chapter.ReleaseDate <= DateTime.Now);

        public override async Task<IEnumerable<TChapter>> GetRange(int skip, int take) => await GetRange(skip, take, true);

        public Task<IEnumerable<TChapter>> GetRange(int skip, int take, bool includeHidden) => Task.FromResult<IEnumerable<TChapter>>(includeHidden
            ? DbSet.OrderByDescending(chapter => chapter.ReleaseDate).Skip(skip).Take(take)
            : DbSet.Where(chapter => chapter.Visible && chapter.ReleaseDate <= DateTime.Now).OrderByDescending(chapter => chapter.ReleaseDate).Skip(skip).Take(take));

        public Task<IEnumerable<TChapter>> GetRange(Title title, int skip, int take, bool includeHidden = false) => Task.FromResult<IEnumerable<TChapter>>(includeHidden
            ? DbSet.Where(chapter => chapter.TitleId == title.Id).OrderByDescending(chapter => chapter.Number).Skip(skip).Take(take)
            : DbSet.Where(chapter => chapter.TitleId == title.Id && chapter.Visible && chapter.ReleaseDate <= DateTime.Now).OrderByDescending(chapter => chapter.Number).Skip(skip).Take(take));

        public override async Task<IEnumerable<TChapter>> GetAll() => await GetAll(true);

        public Task<IEnumerable<TChapter>> GetAll(bool includeHidden) => Task.FromResult<IEnumerable<TChapter>>(includeHidden
            ? DbSet.OrderByDescending(chapter => chapter.ReleaseDate)
            : DbSet.Where(chapter => chapter.Visible && chapter.ReleaseDate <= DateTime.Now).OrderByDescending(chapter => chapter.ReleaseDate));
        
        public Task<IEnumerable<TChapter>> GetAll(Title title, bool includeHidden = false) => Task.FromResult<IEnumerable<TChapter>>(includeHidden
            ? DbSet.Where(chapter => chapter.TitleId == title.Id).OrderByDescending(chapter => chapter.Number)
            : DbSet.Where(chapter => chapter.TitleId == title.Id && chapter.Visible).OrderByDescending(chapter => chapter.Number));
    }
}