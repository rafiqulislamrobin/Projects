using Microsoft.EntityFrameworkCore;
using StockData.Data;
using StockData.info.Context;
using StockData.info.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info.Repositories
{
    public class CompanyRepository : Repository<Company, int>, ICompanyRepository
    {
        public CompanyRepository(IStockDataDbContext context)
    : base((DbContext)context)
        {

        }
    }
}
