using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EO = StockData.info.Entities;
using BO = StockData.info.BusinessObject;
namespace StockData.info.Profiles
{

    class StockPriceProfile : Profile
    {
        public StockPriceProfile()
        {
            CreateMap<EO.StockPrice, BO.StockPrice>().ReverseMap();

        }
    }
}
