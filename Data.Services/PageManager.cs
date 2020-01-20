using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class PageManager<TPage> : GenericManager<TPage> where TPage : Page
    {
        public PageManager(BaseContext context) : base(context) { }

        public async Task<long> CountPagesByChapterId(Guid chapterId) => await DbSet.LongCountAsync(page => page.ChapterId == chapterId);

#pragma warning disable 1998
        public async Task<IEnumerable<TPage>> GetPagesByChapterId(Guid chapterId) => DbSet.Where(page => page.ChapterId == chapterId);
#pragma warning restore 1998
    }
}