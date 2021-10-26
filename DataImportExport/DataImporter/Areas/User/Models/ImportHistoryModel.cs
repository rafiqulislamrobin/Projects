using Autofac;
using DataImporter.Info.Services;
using DataImporter.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataImporter.Areas.User.Models
{

    public class ImportHistoryModel
    {
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }


        private  IDataImporterService _iDataImporterService;
        private  IHttpContextAccessor _httpContextAccessor;
        private ILifetimeScope _scope;
        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;
            _iDataImporterService = _scope.Resolve<IDataImporterService>();
            _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
        }
        public ImportHistoryModel()
        {

        }
        public ImportHistoryModel(IDataImporterService iDataImporterService, IHttpContextAccessor httpContextAccessor)
        {
            _iDataImporterService = iDataImporterService;
            _httpContextAccessor = httpContextAccessor;
        }


        internal object GetHistories(DataTablesAjaxRequestModel dataTableAjaxRequestModel)
        {
            var id = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = _iDataImporterService.GetImporthistory(
                dataTableAjaxRequestModel.PageIndex,
                dataTableAjaxRequestModel.PageSize,
                dataTableAjaxRequestModel.SearchText,
                dataTableAjaxRequestModel.GetSortText(new string[] { "FileName", "DateTime", "GroupName", "FileStatus" }),
                id,DateFrom,DateTo);
            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = (from record in data.records
                        select new string[]
                        {
                                record.FileName.ToString(),
                                record.DateTime.ToString(),
                                record.GroupName.ToString(),
                                record.FileStatus.ToString()
                        }).ToArray()
            };

        }
    }
}
