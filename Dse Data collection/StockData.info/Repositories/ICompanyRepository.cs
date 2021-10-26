using StockData.Data;
using StockData.info.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info.Repositories
{

    public interface ICompanyRepository : IRepository<Company, int>
    {
    }
}
