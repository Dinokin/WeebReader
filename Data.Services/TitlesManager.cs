﻿using System;
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

        public async Task<IEnumerable<TTitle>> Search(string term, bool includeHidden)
        {
            var titles = new List<TTitle>();
            
            titles.AddRange(await SearchByName(term, includeHidden));
            titles.AddRange(await SearchByTag(term, includeHidden));

            return titles.Distinct().OrderBy(title => title.Name);
        }
        
        public async Task<long> Count(bool includeHidden) => includeHidden
            ? await base.Count()
            : await DbSet.LongCountAsync(title => title.Visible);

        public override async Task<IEnumerable<TTitle>> GetRange(int skip, int take) => await GetRange(skip, take, true);

        public Task<IEnumerable<TTitle>> GetRange(int skip, int take, bool includeHidden) => Task.FromResult<IEnumerable<TTitle>>(includeHidden
            ? DbSet.OrderBy(title => title.Name).Skip(skip).Take(take)
            : DbSet.Where(title => title.Visible).OrderBy(title => title.Name).Skip(skip).Take(take));

        public override async Task<IEnumerable<TTitle>> GetAll() => await GetAll(true);

        public Task<IEnumerable<TTitle>> GetAll(bool includeHidden) => Task.FromResult<IEnumerable<TTitle>>(includeHidden
            ? DbSet.OrderBy(title => title.Name)
            : DbSet.Where(title => title.Visible).OrderBy(title => title.Name));

        public async Task<IEnumerable<Tag>> GetTags(TTitle title)
        {
            var targetTitle = await DbSet.Include(entity => entity.TitleTags).SingleOrDefaultAsync(entity => entity == title);

            return targetTitle == null ? new Tag[0] : targetTitle.TitleTags.Join(Context.Tags, titleTag => titleTag.TagId, tag => tag.Id, (titleTag, tag) => tag);
        }

        public async Task<bool> Add(TTitle title, IEnumerable<string>? tags = null)
        {
            title.Version = 1;
            
            if (!await base.Add(title))
                return false;

            if (tags != null)
                await Context.TitleTags.AddRangeAsync((await BuildTags(tags)).Select(tag => new TitleTag(title.Id, tag.Id)));

            return await Context.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> Edit(TTitle title, IEnumerable<string>? tags = null)
        {
            title.Version++;
            
            if (!await base.Edit(title))
                return false;
            
            if (Context.TitleTags.Where(titleTag => titleTag.Title == title) is var titleTags && titleTags.Any())
                Context.TitleTags.RemoveRange(Context.TitleTags.Where(titleTag => titleTag.Title == title));

            if (tags != null)
                await Context.TitleTags.AddRangeAsync((await BuildTags(tags)).Select(tag => new TitleTag(title.Id, tag.Id)));
            
            Context.Tags.RemoveRange(GetUnusedTags());

            return await Context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> Delete(TTitle entity)
        {
            var result = await base.Delete(entity);

            if (result)
                Context.RemoveRange(GetUnusedTags());

            return result;
        }

        public override async Task<bool> DeleteRange(IEnumerable<TTitle> entities)
        {
            var result = await base.DeleteRange(entities);
            
            if (result)
                Context.RemoveRange(GetUnusedTags());

            return result;
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

        private IEnumerable<Tag> GetUnusedTags() => Context.Tags.Include(tag => tag.TitleTag).Where(tag => !tag.TitleTag.Any());

        private Task<IEnumerable<TTitle>> SearchByName(string name, bool includeHidden) => Task.FromResult<IEnumerable<TTitle>>(includeHidden
            ? DbSet.Where(title => title.Name.Contains(name))
            : DbSet.Where(title => title.Visible && title.Name.Contains(name)));

        private async Task<IEnumerable<TTitle>> SearchByTag(string name, bool includeHidden)
        {
            if (await Context.Tags.SingleOrDefaultAsync(entity => entity.Name == name) is var tag && tag == null)
                return new TTitle[0];

            return includeHidden
                ? DbSet.Join(Context.TitleTags, title => title.Id, titleTag => titleTag.TitleId, (title, titleTag) => new {title, titleTag}).Where(tuple => tuple.titleTag.TagId == tag.Id).Select(tuple => tuple.title)
                : DbSet.Join(Context.TitleTags, title => title.Id, titleTag => titleTag.TitleId, (title, titleTag) => new {title, titleTag}).Where(tuple => tuple.title.Visible && tuple.titleTag.TagId == tag.Id).Select(tuple => tuple.title);;
        }
    }
}