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
        private readonly TagManager _tagManager;

        public TitleManager(BaseContext context) : base(context) => _tagManager = new TagManager(context);
        
        public async Task<long> CountTitlesByTag(string tagName)
        {
            try
            {
                var tag = await Context.Tags.Include(tagJoint => tagJoint.TitleTag).SingleOrDefaultAsync(tagInfo => tagInfo.Name == tagName);

                return tag?.TitleTag.LongCount() ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<IEnumerable<TTitle>> GetTitlesByTag(string tagName, int skip, int take)
        {
            try
            {
                var tag = await Context.Tags.Include(tagJoint => tagJoint.TitleTag).SingleOrDefaultAsync(tagInfo => tagInfo.Name == tagName);

                return tag == null ? new TTitle[0] : tag.TitleTag.Join(DbSet, titleTag => titleTag.TitleId, title => title.Id, (titleTag, title) => title).Skip(skip).Take(take);
            }
            catch
            {
                return new TTitle[0];
            }
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

                tags = tags.ToArray();
                
                foreach (var tag in tags)
                    await _tagManager.AddByName(tag);

                var tagEntities = new List<Tag>();

                foreach (var tag in tags)
                    tagEntities.Add(await _tagManager.GetByName(tag));
                
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