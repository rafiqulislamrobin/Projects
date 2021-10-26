using Microsoft.EntityFrameworkCore;
using StockData.Data;
using StockData.info.Context;
using StockData.info.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info.UnitOfWokr
{
    public class StockDataUnitOfWork : UnitOfWork, IStockDataUnitOfWork
    {
        public ICompanyRepository Company { get; private set; }
        public IStockPriceRepositories StockPrice { get; private set; }

        public StockDataUnitOfWork(IStockDataDbContext context,
             ICompanyRepository company,
             IStockPriceRepositories stockPrice)
              : base((DbContext)context)
        {
            Company = company;
            StockPrice = stockPrice;
        }
    }
}
