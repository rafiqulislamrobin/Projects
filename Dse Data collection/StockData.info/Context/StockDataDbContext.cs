using Microsoft.EntityFrameworkCore;
using StockData.info.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info.Context
{
    public class StockDataDbContext : DbContext, IStockDataDbContext
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;

        public StockDataDbContext(string connectionString ,string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(
                    _connectionString,
                    m => m.MigrationsAssembly(_migrationAssemblyName));
            }

            base.OnConfiguring(dbContextOptionsBuilder);
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Company>()
        //        .HasMany(b => b.StockPrices)
        //        .WithOne(t => t.Company);


        //    base.OnModelCreating(modelBuilder);
        //}

        public DbSet<Company> Companies { get; set; }
        public DbSet<StockPrice> StockPrices { get; set; }
    }
}
