
using DataImporter.Info.Entities;
using DataImporter.MemberShip.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Context
{

    public class DataImporterDbContext : DbContext, IDataImporterDbContext
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;

        public DataImporterDbContext(string connectionString, string migrationAssemblyName)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<ApplicationUser>()
                .ToTable("AspNetUsers", a => a.ExcludeFromMigrations())
                .HasMany<Group>()
                .WithOne(a => a.ApplicationUser);
                
            modelBuilder.Entity<Group>()
                .HasMany(c => c.Contacts)
                .WithOne(g => g.Group);


            // modelBuilder.HasSequence<int>("OrderNumbers", schema: "shared")
            //        .StartsAt(1000)
            //        .IncrementsBy(5);

            //modelBuilder.Entity<Contact>()
            //      .Property(o => o.SequenceId)
            //      .HasDefaultValueSql("NEXT VALUE FOR shared.OrderNumbers");

            modelBuilder.Entity<Group>()
              .HasMany(f => f.FilePaths)
              .WithOne(g => g.Group);


            modelBuilder.Entity<Group>()
              .HasMany(e => e.ExportStatusEntities)
              .WithOne(g=> g.Group);
        }

        public DbSet<FilePath> FilePaths { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ExportStatus> ExportStatus { get; set; }


    }
}
