using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace WeebReader.Data.Services
{
    public class SettingManager : GenericManager<Setting>
    {
        public SettingManager(BaseContext context) : base(context) { }

        public async Task<string> GetValue(string key) => await DbSet.SingleOrDefaultAsync(setting => setting.Key == key) is var result && result != null ? result.Value : string.Empty;

        public async Task<T> GetValue<T>(string key) => (T) Convert.ChangeType((await DbSet.SingleOrDefaultAsync(setting => setting.Key == key))?.Value, typeof(T));
    }
}