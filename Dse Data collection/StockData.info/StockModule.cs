using Autofac;
using Microsoft.Extensions.Configuration;
using StockData.info.Context;
using StockData.info.Repositories;
using StockData.info.Services;
using StockData.info.UnitOfWokr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info
{
    public class StockModule : Module
    {
        private readonly string _connectionString;
        private readonly string _migrationAssemblyName;
   

        public StockModule(string connectionString, string migrationAssemblyName)
        {
            _connectionString = connectionString;
            _migrationAssemblyName = migrationAssemblyName;


        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StockDataDbContext>()
                .AsSelf()
                .WithParameter("connectionString", _connectionString)
               .WithParameter("migrationAssemblyName", _migrationAssemblyName)
               .SingleInstance();

            builder.RegisterType<StockDataDbContext>().As<IStockDataDbContext>()
                .WithParameter("connectionString", _connectionString)
                .WithParameter("migrationAssemblyName", _migrationAssemblyName)
                .InstancePerLifetimeScope();


            builder.RegisterType<CompanyRepository>().As<ICompanyRepository>()
               .InstancePerLifetimeScope();
            builder.RegisterType<StockService>().As<IStockService>()
               .InstancePerLifetimeScope();
            builder.RegisterType<StockDataUnitOfWork>().As<IStockDataUnitOfWork>()
               .InstancePerLifetimeScope();

            builder.RegisterType<StockPriceRepositories>().As<IStockPriceRepositories>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
