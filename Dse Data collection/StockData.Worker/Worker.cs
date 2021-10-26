using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockData.info.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockData.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        
        private readonly IStockService _iStockService;
        public Worker(ILogger<Worker> logger ,IStockService iStockService)
        {
            _logger = logger;
            //AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            _iStockService = iStockService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _iStockService.addStockPrice();
               
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60*1000, stoppingToken);
            }
        }
    }
}
