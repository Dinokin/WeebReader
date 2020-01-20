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

#pragma warning disable 1998
        public virtual async Task<IEnumerable<TEntity>> GetRange(int skip, int take) => DbSet.Skip(skip).Take(take);
#pragma warning restore 1998

        public virtual async Task<bool> Add(TEntity entity)
        {
            try
            {
                await DbSet.AddAsync(entity);

                return await Context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> Edit(TEntity entity)
        {
            try
            { 
                DbSet.Update(entity);

                return await Context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public virtual async Task<bool> Delete(Guid id)
        {
            try
            { 
                DbSet.Remove(await DbSet.SingleAsync(entity => entity.Id == id));

                return await Context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}