using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace WeebReader.Data.Contexts.Factories
{
    internal class MariaDbContextFactory : IDesignTimeDbContextFactory<MariaDbContext>
    {
        public MariaDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder();
            options.UseMySql("Server=localhost;Database=WeebDb;Uid=root;Pwd=123456;", builder => builder.ServerVersion(ServerVersion.Default.Version, ServerType.MariaDb));
            
            return new MariaDbContext(options.Options);
        }
    }
}