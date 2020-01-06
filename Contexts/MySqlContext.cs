using Microsoft.EntityFrameworkCore;
using WeebReader.Data.Contexts.Abstract;

namespace WeebReader.Data.Contexts
{
    public class MySqlContext : BaseContext
    {
        public MySqlContext(DbContextOptions options) : base(options) { }
        
        
    }
}