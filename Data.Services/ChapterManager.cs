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

        public async Task<long> CountChaptersByTitleId(Guid titleId) => await DbSet.LongCountAsync(chapter => chapter.TitleId == titleId);

#pragma warning disable 1998
        public async Task<IEnumerable<TChapter>> GetChaptersByTitleId(Guid titleId, int skip, int take) => DbSet.Where(chapter => chapter.TitleId == titleId).Skip(skip).Take(take);
#pragma warning restore 1998
    }
}