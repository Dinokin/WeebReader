using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class ParametersManager : GenericManager<Parameter>
    {
        public ParametersManager(BaseContext context) : base(context) { }

        public async Task<T> GetValue<T>(ushort type)
        {
            if ((await DbSet.SingleOrDefaultAsync(parameter => parameter.Type == type))?.Value is var value && value != null)
                return (T) Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));

            return default!;
        }

        public async Task<bool> Save(ushort type, string? value)
        {
            var parameter = await DbSet.SingleOrDefaultAsync(entity => entity.Type == type);

            if (parameter != null)
            {
                parameter.Value = value;
                
                return await Edit(parameter);
            }
            
            parameter = new Parameter(type, value);

            return await Add(parameter);
        }
    }
}