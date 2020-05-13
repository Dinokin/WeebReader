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
        
        public async Task<long> Count(bool includeHidden) => includeHidden
            ? await base.Count()
            : await DbSet.LongCountAsync(post => post.Visible);

        public override async Task<IEnumerable<Post>> GetRange(int skip, int take) => await GetRange(skip, take, true);

        public Task<IEnumerable<Post>> GetRange(int skip, int take, bool includeHidden) => Task.FromResult<IEnumerable<Post>>(includeHidden
            ? DbSet.OrderByDescending(post => post.ReleaseDate).Skip(skip).Take(take)
            : DbSet.Where(post => post.Visible).OrderByDescending(post => post.ReleaseDate).Skip(skip).Take(take));

        public override async Task<IEnumerable<Post>> GetAll() => await GetAll(true);

        public Task<IEnumerable<Post>> GetAll(bool includeHidden) => Task.FromResult<IEnumerable<Post>>(includeHidden
            ? DbSet.OrderByDescending(post => post.ReleaseDate)
            : DbSet.Where(post => post.Visible).OrderByDescending(post => post.ReleaseDate));
    }
}