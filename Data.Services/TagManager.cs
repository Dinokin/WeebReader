using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class TagManager : GenericManager<Tag>
    {
        public TagManager(BaseContext context) : base(context) { }

        public async Task<Tag> GetByName(string name) => await DbSet.SingleOrDefaultAsync(tag => tag.Name == name);

        public async Task<bool> AddByName(string name)
        {
            var tag = new Tag
            {
                Name = name
            };

            return await base.Add(tag);
        }

        public async Task<bool> RemoveByName(string name)
        {
            var target = await DbSet.SingleOrDefaultAsync(tag => tag.Name == name);

            return target != null && await base.Delete(target.Id);
        }
    }
}