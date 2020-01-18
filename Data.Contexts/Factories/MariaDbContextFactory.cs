using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WeebReader.Data.Contexts.Factories
{
    internal class MariaDbContextFactory : IDesignTimeDbContextFactory<MariaDbContext>
    {
        public MariaDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder();
            options.UseMySql("Server=localhost;Database=WeebDb;Uid=root;Pwd=123456;");
            
            return new MariaDbContext(options.Options);
        }
    }
}