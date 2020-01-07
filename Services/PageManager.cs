using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace Services
{
    public class PageManager<TPage> : BaseService where TPage : Page
    {
        private readonly DbSet<TPage> _pages;

        public PageManager(BaseContext context) : base(context)
        {
            _pages = (DbSet<TPage>) Context.GetType().GetProperties().Single(prop => prop.PropertyType == typeof(TPage)).GetValue(Context);
        }
    }
}