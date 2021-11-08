using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tamaris.DAL.DbContexts
{
    public class TamarisDesignDbContext : IDesignTimeDbContextFactory<TamarisDbContext>
    {
        public TamarisDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TamarisDbContext>();
            optionsBuilder.UseSqlServer("SQLSERVER_CONNECTION_STRING");

            return new TamarisDbContext(optionsBuilder.Options);
        }
    }
}

