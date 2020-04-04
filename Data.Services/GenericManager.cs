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
        
        public IQueryable<TEntity> Entities { get; }

        public GenericManager(BaseContext context)
        {
            Context = context;
            DbSet = (DbSet<TEntity>) typeof(BaseContext).GetProperties().Single(info => info.PropertyType == typeof(DbSet<TEntity>)).GetValue(Context)!;
            Entities = DbSet.AsQueryable();
        }
        
        public virtual async Task<long> Count() => await DbSet.LongCountAsync();

        public virtual async Task<TEntity> GetById(Guid id) => await DbSet.SingleOrDefaultAsync(entity => entity.Id == id);

        public virtual Task<IEnumerable<TEntity>> GetRange(int skip, int take) => Task.FromResult<IEnumerable<TEntity>>(DbSet.Skip(skip).Take(take));

        public virtual async Task<bool> Add(TEntity entity)
        {
            await DbSet.AddAsync(entity);

            return await Context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> AddRange(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
            
            return await Context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Edit(TEntity entity)
        {
            DbSet.Update(entity);

            return await Context.SaveChangesAsync() > 0;
        }
        
        public virtual async Task<bool> EditRange(IEnumerable<TEntity> entities)
        { 
            DbSet.UpdateRange(entities);
            
            return await Context.SaveChangesAsync() > 0;
        } 

        public virtual async Task<bool> Delete(TEntity entity)
        {
            DbSet.Remove(entity);

            return await Context.SaveChangesAsync() > 0;
        }
        
        public virtual async Task<bool> DeleteRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);

            return await Context.SaveChangesAsync() > 0;
        }
    }
}