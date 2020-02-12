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
            var target = await Context.Tags.Include(tagJoint => tagJoint.TitleTag).SingleOrDefaultAsync(tagInfo => tagInfo.Id == tag.Id);

            return target == null ? new TTitle[0] : target.TitleTag.Join(DbSet, titleTag => titleTag.TitleId, title => title.Id, (titleTag, title) => title).Skip(skip).Take(take);
        }

        public async Task<bool> Add(TTitle title, IEnumerable<string> tags)
        {
            await DbSet.AddAsync(title);

            var tagEntities = tags.Select(s => new Tag { Name = s }).ToArray();
                
            for (var i = 0; i < tagEntities.Length; i++)
                if (await Context.Tags.SingleOrDefaultAsync(tagEntity => tagEntity.Name == tagEntities[i].Name) is var entity && entity == null)
                    await Context.Tags.AddAsync(tagEntities[i]);
                else
                    tagEntities[i] = entity;

            await Context.TitleTags.AddRangeAsync(tagEntities.Select(entity => new TitleTag { TagId = entity.Id, TitleId = title.Id }));

            return await Context.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> Edit(TTitle title, IEnumerable<string> tags)
        {
            var target = await DbSet.Include(entity => entity.TitleTags).SingleOrDefaultAsync(entity => entity.Id == title.Id);

            if (target == null)
                return false;

            title.Id = target.Id;
            DbSet.Update(title);
            Context.TitleTags.RemoveRange(title.TitleTags);
            
            var tagEntities = tags.Select(s => new Tag { Name = s }).ToArray();
                
            for (var i = 0; i < tagEntities.Length; i++)
                if (await Context.Tags.SingleOrDefaultAsync(tagEntity => tagEntity.Name == tagEntities[i].Name) is var entity && entity == null)
                    await Context.Tags.AddAsync(tagEntities[i]);
                else
                    tagEntities[i] = entity;

            await Context.TitleTags.AddRangeAsync(tagEntities.Select(entity => new TitleTag { TagId = entity.Id, TitleId = title.Id }));

            return await Context.SaveChangesAsync() > 0;
        }
    }
}