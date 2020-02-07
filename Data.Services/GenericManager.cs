using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Services
{
    public class GenericManager<TEntity> where TEntity : BaseEntity
    {
        protected readonly BaseContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericManager(BaseContext context)
        {
            Context = context;
            DbSet = (DbSet<TEntity>) typeof(BaseContext).GetProperties().Single(info => info.PropertyType == typeof(DbSet<TEntity>)).GetValue(Context);
        }

        public virtual async Task<long> Count() => await DbSet.LongCountAsync();

        public virtual async Task<TEntity> GetById(Guid id) => await DbSet.SingleOrDefaultAsync(entity => entity.Id == id);

        public virtual Task<IEnumerable<TEntity>> GetRange(int skip, int take) => Task.FromResult<IEnumerable<TEntity>>(DbSet.Skip(skip).Take(take));

        public virtual async Task<bool> Add(TEntity entity)
        {
            await DbSet.AddAsync(entity);

            return await Context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Edit(TEntity entity)
        {
            DbSet.Update(entity);

            return await Context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Delete(Guid id)
        {
            DbSet.Remove(await DbSet.SingleAsync(entity => entity.Id == id));

            return await Context.SaveChangesAsync() > 0;
        }
    }
}