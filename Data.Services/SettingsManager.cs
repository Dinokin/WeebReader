using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class SettingsManager : GenericManager<Setting>
    {
        public SettingsManager(BaseContext context) : base(context) { }

        public async Task<string> GetValue(Setting.Keys key) => await DbSet.SingleOrDefaultAsync(setting => setting.Key == key) is var result && result != null ? result.Value : string.Empty;

        public async Task<T> GetValue<T>(Setting.Keys key)
        {
            if ((await DbSet.SingleOrDefaultAsync(setting => setting.Key == key))?.Value is var value && value != null)
                return (T) Convert.ChangeType(value, typeof(T));

            return default;
        }
    }
}