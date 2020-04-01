using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class ParameterManager : GenericManager<Parameter>
    {
        public ParameterManager(BaseContext context) : base(context) { }
        
        public async Task<T> GetValue<T>(Parameter.Types type)
        {
            if ((await DbSet.SingleOrDefaultAsync(parameter => parameter.Type == type))?.Value is var value && value != null)
                return (T) Convert.ChangeType(value, typeof(T));

            return default!;
        }
    }
}