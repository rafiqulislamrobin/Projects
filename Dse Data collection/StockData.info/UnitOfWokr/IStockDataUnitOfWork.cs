using StockData.Data;
using StockData.info.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info.UnitOfWokr
{
    public interface IStockDataUnitOfWork : IUnitOfWork
    {
        ICompanyRepository Company { get; }
        IStockPriceRepositories StockPrice { get; }
    }
}
