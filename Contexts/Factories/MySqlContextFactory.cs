using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WeebReader.Data.Contexts.Factories
{
    public class MySqlContextFactory : IDesignTimeDbContextFactory<MySqlContext>
    {
        public MySqlContext CreateDbContext(string[] args)
        {
            return new MySqlContext(new DbContextOptionsBuilder().UseMySql("Server=localhost;Database=ef;User=root;Password=123456;").Options);
        }
    }
}