using DataImporter.Common.Utility;
using DataImporter.Info.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataImporter.ImportWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDataImporterService _iDataImporterService;
      
        public Worker(ILogger<Worker> logger, IDataImporterService iDataImporterService)
        {
            _iDataImporterService = iDataImporterService;
            _logger = logger;
           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
               var message = _iDataImporterService.SaveExcelDatatoDb();
                _logger.LogInformation(message);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken);
            }

        }
    }
}
