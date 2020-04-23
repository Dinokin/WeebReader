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

        public async Task<long> Count(Title title, bool includeHidden = false)
        {
            if (includeHidden)
                return await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id);
            
            return await DbSet.LongCountAsync(chapter => chapter.TitleId == title.Id && chapter.Visible);
        }

        public Task<IEnumerable<TChapter>> GetRange(int skip, int take, bool includeHidden = false) => Task.FromResult<IEnumerable<TChapter>>(includeHidden
            ? DbSet.OrderByDescending(chapter => chapter.ReleaseDate).Skip(skip).Take(take)
            : DbSet.Join(Context.Titles, chapter => chapter.TitleId, title => title.Id, (chapter, title) => new {title, chapter})
                .Where(tuple => tuple.title.Visible && tuple.chapter.Visible && tuple.chapter.ReleaseDate <= DateTime.Now)
                .Select(tuple => tuple.chapter).OrderByDescending(chapter => chapter.ReleaseDate).Skip(skip).Take(take));

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