using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class PostManager : GenericManager<Post>
    {
        public PostManager(BaseContext context) : base(context) { }

        public override Task<IEnumerable<Post>> GetRange(int skip, int take) => Task.FromResult<IEnumerable<Post>>(DbSet.OrderByDescending(post => post.ReleaseDate).Skip(skip).Take(take));
    }
}