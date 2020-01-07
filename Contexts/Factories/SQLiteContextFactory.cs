﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WeebReader.Data.Contexts.Factories
{
    internal class SQLiteContextFactory : IDesignTimeDbContextFactory<SQLiteContext>
    {
        private string ConnectionString { get; }

        public SQLiteContextFactory() => ConnectionString = "Data Source = database.db;";
        
        public SQLiteContext CreateDbContext(string[] args) => new SQLiteContext(new DbContextOptionsBuilder().UseSqlite(ConnectionString).Options);
    }
}