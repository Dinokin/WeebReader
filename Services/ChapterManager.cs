using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities.Abstract;

namespace Services
{
    public class ChapterManager<TChapter> : BaseService where TChapter : Chapter
    {
        private readonly DbSet<TChapter> _chapters;

        public ChapterManager(BaseContext context) : base(context)
        {
            _chapters = (DbSet<TChapter>) Context.GetType().GetProperties().Single(prop => prop.PropertyType == typeof(TChapter)).GetValue(Context);
        }
    }
}