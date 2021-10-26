using Autofac;
using DataImporter.Info.Context;
using DataImporter.Info.Repositories;
using DataImporter.Info.Services;
using DataImporter.Info.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info
{

    public class DataImporterModule : Module
    {


        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;

        public DataImporterModule(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;

        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataImporterDbContext>()
              .AsSelf()
              .WithParameter("connectionString", _connectionString)
              .WithParameter("migrationAssemblyName", _migrationAssemblyName)
              .InstancePerLifetimeScope();

            builder.RegisterType<DataImporterDbContext>().As<IDataImporterDbContext>()
                .WithParameter("connectionString", _connectionString)
                .WithParameter("migrationAssemblyName", _migrationAssemblyName)
                .InstancePerLifetimeScope();

            builder.RegisterType<FilePathRepository>().As<IFilePathRepository>()
              .InstancePerLifetimeScope();

           
            builder.RegisterType<DataImporterService>().As<IDataImporterService>()
               .InstancePerLifetimeScope();
            builder.RegisterType<GroupServices>().As<IGroupServices>()
               .InstancePerLifetimeScope();
            builder.RegisterType<ExportServices>().As<IExportServices>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DataUnitOfWork>().As<IDataUnitOfWork>()
               .InstancePerLifetimeScope();

            builder.RegisterType<GroupRepository>().As<IGroupRepository>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<ContactRepository>().As<IContactRepository>()
              .InstancePerLifetimeScope();
            builder.RegisterType<ExportStatusRepository> ().As<IExportStatusRepository>()
              .InstancePerLifetimeScope();

            base.Load(builder);

        }
    }
}
