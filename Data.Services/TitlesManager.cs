using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class TitlesManager<TTitle> : GenericManager<TTitle> where TTitle : Title
    { 
        public TitlesManager(BaseContext context) : base(context) { }
        
        public async Task<long> Count(bool includeHidden = false)
        {
            if (includeHidden)
                return await base.Count();

            return await DbSet.LongCountAsync(title => title.Visible);
        }

        public Task<IEnumerable<TTitle>> GetRange(int skip, int take, bool includeHidden = false) => Task.FromResult<IEnumerable<TTitle>>(includeHidden ?
            DbSet.OrderBy(title => title.Name).Skip(skip).Take(take) :
            DbSet.Where(title => title.Visible).Skip(skip).Take(take));

        public async Task<IEnumerable<Tag>> GetTags(TTitle title)
        {
            var targetTitle = await DbSet.Include(entity => entity.TitleTags).SingleOrDefaultAsync(entity => entity == title);

            return targetTitle == null ? new Tag[0] : targetTitle.TitleTags.Join(Context.Tags, titleTag => titleTag.TagId, tag => tag.Id, (titleTag, tag) => tag);
        }

        public async Task<bool> Add(TTitle title, IEnumerable<string> tags)
        {
            await DbSet.AddAsync(title);
            
            await Context.TitleTags.AddRangeAsync((await BuildTags(tags)).Select(tag => new TitleTag(title.Id, tag.Id)));

            return await Context.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> Edit(TTitle title, IEnumerable<string> tags)
        {
            DbSet.Update(title);
            
            if (Context.TitleTags.Where(titleTag => titleTag.Title == title) is var titleTags && titleTags.Any())
                Context.TitleTags.RemoveRange(Context.TitleTags.Where(titleTag => titleTag.Title == title));
            
            await Context.TitleTags.AddRangeAsync((await BuildTags(tags)).Select(tag => new TitleTag(title.Id, tag.Id)));

            return await Context.SaveChangesAsync() > 0;
        }

        private async Task<IEnumerable<Tag>> BuildTags(IEnumerable<string> tags)
        {
            var tagEntities = tags.Select(name => new Tag(name)).ToArray();
            
            for (var i = 0; i < tagEntities.Length; i++)
                if (await Context.Tags.SingleOrDefaultAsync(tag => tag.Name == tagEntities[i].Name) is var entity && entity == null)
                    await Context.Tags.AddAsync(tagEntities[i]);
                else
                    tagEntities[i] = entity;

            return tagEntities;
        }
    }
}