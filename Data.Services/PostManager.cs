using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class PostsManager : GenericManager<Post>
    {
        public PostsManager(BaseContext context) : base(context) { }
        
        public async Task<long> Count(bool includeHidden = false)
        {
            if (includeHidden)
                return await base.Count();

            return await DbSet.LongCountAsync(post => post.Visible);
        }

        public Task<IEnumerable<Post>> GetRange(int skip, int take, bool includeHidden = false) => Task.FromResult<IEnumerable<Post>>(includeHidden ?
            DbSet.OrderByDescending(post => post.ReleaseDate).Skip(skip).Take(take) :
            DbSet.Where(post => post.Visible).OrderByDescending(post => post.ReleaseDate).Skip(skip).Take(take));
    }
}