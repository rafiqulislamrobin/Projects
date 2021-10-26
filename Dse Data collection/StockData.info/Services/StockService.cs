using AutoMapper;
using HtmlAgilityPack;
using StockData.info.BusinessObject;
using StockData.info.Context;
using StockData.info.UnitOfWokr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockData.info.Services
{
    public class StockService : IStockService
    {

        
        private readonly IStockDataUnitOfWork _iStockDataUnitOfWork;
        private readonly IMapper _mapper;

        public StockService(IStockDataUnitOfWork iStockDataUnitOfWork)
        {
            _iStockDataUnitOfWork = iStockDataUnitOfWork;
            
        }
        public void addStockPrice()
        {
            HtmlWeb web = new HtmlWeb();


            HtmlDocument doc = web.Load("https://www.dse.com.bd/latest_share_price_scroll_l.php");

            var hlw = doc.DocumentNode.SelectSingleNode("//span[@class='green']");
            if (hlw.InnerText != "Closed")
            {
                List<StockPrice> stocks = new List<StockPrice>();

                StockPrice stock = new();
                Company company = new();

                var aNodes2 = doc.DocumentNode.SelectSingleNode("//table[@class='table table-bordered background-white shares-table fixedHeader']");
                HtmlNode[] nodes = aNodes2.SelectNodes(".//tr//td").ToArray();

                for (int i = 0; i < 4180; i++)
                {
                    var nodes2 = nodes[i].InnerText;
                    if (i % 11 == 0 || i == 0)
                    {
                        stock.CompanyId = Convert.ToInt32((nodes2));
                    }
                    else if (i % 11 == 1)
                    {
                        company.TradeCode = ((nodes2));
                    }
                    else if (i % 11 == 2)
                    {
                        stock.LastTradingPrice = Convert.ToDouble((nodes2));

                    }
                    else if (i % 11 == 3)
                    {
                        stock.High = Convert.ToDouble((nodes2));
                    }
                    else if (i % 11 == 4)
                    {
                        stock.Low = Convert.ToDouble((nodes2));
                    }
                    else if (i % 11 == 5)
                    {
                        stock.ClosePrice = Convert.ToDouble((nodes2));
                    }
                    else if (i % 11 == 6)
                    {
                        stock.YesterdayClosePrice = Convert.ToDouble((nodes2));
                    }

                    else if (i % 11 == 7)
                    {

                        if (nodes2 != "--")
                        {

                            stock.Change = Convert.ToDouble((nodes2));
                        }
                        else
                        {
                            stock.LastTradingPrice = 0;
                        }
                    }
                    else if (i % 11 == 8)
                    {
                        stock.Trade = Convert.ToDouble((nodes2));
                    }
                    else if (i % 11 == 9)
                    {
                        stock.Value = Convert.ToDouble((nodes2));
                    }
                    else if (i % 11 == 10)
                    {
                        stock.Volume = Convert.ToDouble((nodes2));
                    }


                    if (i % 11 == 10)
                    {
                        stocks.Add(stock);
                       
                       
                        _iStockDataUnitOfWork.StockPrice.Add(new Entities.StockPrice
                        {

                            CompanyId = stock.CompanyId,
                            LastTradingPrice = stock.LastTradingPrice,
                            High = stock.High,
                            Low = stock.Low,
                            ClosePrice = stock.ClosePrice,

                            YesterdayClosePrice = stock.YesterdayClosePrice,
                            Change = stock.Change,
                            Trade = stock.Trade,
                            Value = stock.Value,
                            Volume = stock.Volume


                        });
                        _iStockDataUnitOfWork.Save();
                        stock = new();
                    }
                    else if(i %11 ==1)
                    {
                        _iStockDataUnitOfWork.Company.Add(new Entities.Company
                        {
                            TradeCode=company.TradeCode
                        });
                        _iStockDataUnitOfWork.Save();
                        company = new();
                    }

                }
                
            }
        }

     
    }
}
