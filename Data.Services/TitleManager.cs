using System;
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

        public async Task<IEnumerable<TTitle>> GetTitlesByTag(string tagName, int skip, int take)
        {
            var tag = await Context.Tags.Include(tagJoint => tagJoint.TitleTag).SingleOrDefaultAsync(tagInfo => tagInfo.Name == tagName);

            return tag == null ? new TTitle[0] : tag.TitleTag.Join(DbSet, titleTag => titleTag.TitleId, title => title.Id, (titleTag, title) => title).Skip(skip).Take(take);
        }
        
        public async Task<bool> TagTitle(Guid titleId, IEnumerable<string> tags)
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                var title = await DbSet.Include(entity => entity.TitleTags).SingleOrDefaultAsync(entity => entity.Id == titleId);

                if (title == null)
                    return false;
                
                Context.TitleTags.RemoveRange(title.TitleTags);

                var tagEntities = tags.Select(s => new Tag
                {
                    Name = s
                }).ToArray();

                for (var i = 0; i < tagEntities.Length; i++)
                {
                    if (await Context.Tags.SingleOrDefaultAsync(tagEntity => tagEntity.Name == tagEntities[i].Name) is var entity && entity == null)
                        await Context.Tags.AddAsync(tagEntities[i]);
                    else
                        tagEntities[i] = entity;
                }

                var titleTags = tagEntities.Select(entity => new TitleTag {TagId = entity.Id, TitleId = titleId});

                await Context.TitleTags.AddRangeAsync(titleTags);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                
                return false;
            }
        }
    }
}