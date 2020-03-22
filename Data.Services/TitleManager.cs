using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class TitleManager<TTitle> : GenericManager<TTitle> where TTitle : Title
    { 
        public TitleManager(BaseContext context) : base(context) { }
        
        public async Task<long> CountTitlesByTag(string tagName)
        {
            var tag = await Context.Tags.Include(tagJoint => tagJoint.TitleTag).SingleOrDefaultAsync(tagInfo => tagInfo.Name == tagName);

            return tag?.TitleTag.LongCount() ?? 0;
        }

        public async Task<IEnumerable<TTitle>> GetTitlesByTag(Tag tag, int skip, int take)
        {
            var targetTag = await Context.Tags.Include(tagJoint => tagJoint.TitleTag).SingleOrDefaultAsync(tagInfo => tagInfo.Id == tag.Id);

            return targetTag == null ? new TTitle[0] : targetTag.TitleTag.Join(DbSet, titleTag => titleTag.TitleId, title => title.Id, (titleTag, title) => title)
                .OrderBy(title => title.Name).Skip(skip).Take(take);
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
            
            if (Context.TitleTags.Where(titleTag => titleTag.TitleId == title.Id) is var titleTags && titleTags.Any())
                Context.TitleTags.RemoveRange(title.TitleTags);
            
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