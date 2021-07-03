using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Web.API.Data.Contexts.Abstract;
using WeebReader.Web.API.Data.Entities.Abstract;

namespace WeebReader.Web.API.Data.DAOs
{
    public class GenericDAO<TEntity> where TEntity : BaseEntity
    {
        protected readonly BaseContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericDAO(BaseContext context)
        {
            Context = context;
            DbSet = (DbSet<TEntity>) typeof(BaseContext).GetProperties()
                .Single(info => info.PropertyType == typeof(DbSet<TEntity>)).GetValue(Context)!;
        }

        public virtual async Task<ulong> Count() => (ulong) await DbSet.LongCountAsync();
        
        public virtual Task<IEnumerable<TEntity>> GetRange(ushort skip, ushort take) => Task.FromResult(DbSet.Skip(skip).Take(take).AsEnumerable());

        public virtual async Task<TEntity?> GetById(Guid id) => await DbSet.SingleOrDefaultAsync(entity => entity.Id == id);

        public virtual async Task Add(TEntity entity)
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task Edit(TEntity entity)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task Delete(TEntity entity)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
    }
}