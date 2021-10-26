using Autofac;
using DataImporter.Common.Utility;
using DataImporter.Info.Business_Object;
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
    public class ExportHistoryModel
    {
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }

        private  IHttpContextAccessor _httpContextAccessor;
        private  IExportServices _exportServices;
        private ILifetimeScope _scope;
        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;
            _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
            _exportServices = _scope.Resolve<IExportServices>();
        }
        public ExportHistoryModel()
        {

        }
        public ExportHistoryModel(IHttpContextAccessor httpContextAccessor, IExportServices exportServices)
        {

            _httpContextAccessor = httpContextAccessor;
            _exportServices = exportServices;
        }

        internal object GetHistories(DataTablesAjaxRequestModel dataTableAjaxRequestModel)
        {
            var id = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = _exportServices.GetExportHistory(
                    dataTableAjaxRequestModel.PageIndex,
                    dataTableAjaxRequestModel.PageSize,
                    dataTableAjaxRequestModel.SearchText,
                    dataTableAjaxRequestModel.GetSortText(new string[] { "GroupName", "Email", "Id", "DateTime" }),
                      id, DateTo,DateFrom);
            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = (from record in data.records
                        select new string[]
                        {
                                record.GroupName.ToString(),
                               record.Email.ToString(),
                                record.Id.ToString(),
                                record.DateTime.ToString()
                        }
                    ).ToArray()
            };

        }



    }
}
