using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace Services
{
    public class TitleManager<TTitle> : BaseService where TTitle : Title
    {
        private readonly DbSet<TTitle> _titles;

        public TitleManager(BaseContext context) : base(context)
        {
            _titles = (DbSet<TTitle>) Context.GetType().GetProperties().Single(prop => prop.PropertyType == typeof(TTitle)).GetValue(Context);
        }
    }
}