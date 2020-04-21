using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class PagesManager<TPage> : GenericManager<TPage> where TPage : Page
    {
        public PagesManager(BaseContext context) : base(context) { }
        
        public Task<IEnumerable<TPage>> GetAll(Chapter chapter) => Task.FromResult<IEnumerable<TPage>>(DbSet.Where(page => page.ChapterId == chapter.Id));
    }
}