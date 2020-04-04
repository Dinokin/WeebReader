using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class PagesManager<TPage> : GenericManager<TPage> where TPage : Page
    {
        public PagesManager(BaseContext context) : base(context) { }

        public async Task<long> Count(Chapter chapter) => await DbSet.LongCountAsync(page => page.ChapterId == chapter.Id);

        public Task<IEnumerable<TPage>> GetRange(Chapter chapter) => Task.FromResult<IEnumerable<TPage>>(DbSet.Where(page => page.ChapterId == chapter.Id));
    }
}