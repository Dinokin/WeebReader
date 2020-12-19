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

        public Task<IEnumerable<TTitle>> Search(string term, bool includeHidden)
        {
            var results = includeHidden
                ? DbSet.Join(Context.TitleTags, title => title.Id, titleTag => titleTag.TitleId, (title, titleTag) => new {title, titleTag})
                    .Join(Context.Tags, tuple => tuple.titleTag.TagId, tag => tag.Id, (tuple, tag) => new {tuple.title, tag, tuple.titleTag})
                : DbSet.Join(Context.TitleTags, title => title.Id, titleTag => titleTag.TitleId, (title, titleTag) => new {title, titleTag})
                    .Join(Context.Tags, tuple => tuple.titleTag.TagId, tag => tag.Id, (tuple, tag) => new {tuple.title, tag, tuple.titleTag}).Where(tuple => tuple.title.Visible);

            return Task.FromResult<IEnumerable<TTitle>>(results.Where(tuple => tuple.tag.Name.ToLower().Contains(term) || tuple.title.Name.ToLower().Contains(term) || tuple.title.Id.ToString() == term).Select(tuple => tuple.title).Distinct());
        }

        public override async Task<IEnumerable<TTitle>> GetRange(int skip, int take) => await GetRange(skip, take, true);

        public override async Task<IEnumerable<TTitle>> GetAll() => await GetAll(true);

        public Task<IEnumerable<TTitle>> GetAll(bool includeHidden) => Task.FromResult<IEnumerable<TTitle>>(includeHidden
            ? DbSet.OrderBy(title => title.Name)
            : DbSet.Where(title => title.Visible).OrderBy(title => title.Name));

        public async Task<IEnumerable<Tag>> GetTags(TTitle title)
        {
            var targetTitle = await DbSet.Include(entity => entity.TitleTags).SingleOrDefaultAsync(entity => entity == title);

            return targetTitle?.TitleTags?.Join(Context.Tags, titleTag => titleTag.TagId, tag => tag.Id, (_, tag) => tag) ?? new Tag[0];
        }

        public async Task<bool> Add(TTitle title, IEnumerable<string>? tags = null)
        {
            if (!await base.Add(title))
                return false;

            await AddTags(title, tags);

            return true;
        }
        
        public async Task<bool> Edit(TTitle title, IEnumerable<string>? tags = null)
        {
            if (!await base.Edit(title))
                return false;

            await AddTags(title, tags);
            await RemoveUnusedTags();

            return true;
        }

        public override async Task<bool> Delete(TTitle entity)
        {
            var result = await base.Delete(entity);

            if (result)
                await RemoveUnusedTags();

            return result;
        }

        public override async Task<bool> DeleteRange(IEnumerable<TTitle> entities)
        {
            var result = await base.DeleteRange(entities);

            if (result)
                await RemoveUnusedTags();

            return result;
        }
        
        private Task<IEnumerable<TTitle>> GetRange(int skip, int take, bool includeHidden) => Task.FromResult<IEnumerable<TTitle>>(includeHidden
            ? DbSet.OrderBy(title => title.Name).Skip(skip).Take(take)
            : DbSet.Where(title => title.Visible).OrderBy(title => title.Name).Skip(skip).Take(take));
        
        private async Task AddTags(TTitle title, IEnumerable<string>? tags)
        {
            var currentTitleTags = Context.TitleTags.Include(titleTag => titleTag.Tag).Where(titleTag => titleTag.Title == title).ToList();
            var newTags = tags?.Distinct().Select(str => new Tag(str)).ToArray();

            if (newTags != null && newTags.Any())
            {
                for (var i = 0; i < newTags.Length; i++)
                {
                    if (currentTitleTags.SingleOrDefault(titleTag => titleTag.Tag!.Name == newTags[i].Name) is var currentTitleTag && currentTitleTag != null)
                    {
                        currentTitleTags.Remove(currentTitleTag);
                        
                        continue;
                    }

                    if (await Context.Tags.SingleOrDefaultAsync(tag => tag.Name == newTags[i].Name) is var tagEntity && tagEntity != null)
                        await Context.TitleTags.AddAsync(new TitleTag(title.Id, tagEntity.Id));
                    else
                    {
                        await Context.Tags.AddAsync(newTags[i]);
                        await Context.TitleTags.AddAsync(new TitleTag(title.Id, newTags[i].Id));
                    }
                }
            }
            
            Context.TitleTags.RemoveRange(currentTitleTags);
            await Context.SaveChangesAsync();
        }

        private async Task RemoveUnusedTags()
        {
            Context.Tags.RemoveRange(Context.Tags.Include(tag => tag.TitleTag).Where(tag => !tag.TitleTag!.Any()));
            await Context.SaveChangesAsync();
        }
    }
}