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
    public class ViewGroupModel
    {


        private  IHttpContextAccessor _httpContextAccessor;
        private  IGroupServices _groupServices;
        private ILifetimeScope _scope;
        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;

            _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
            _groupServices = _scope.Resolve<IGroupServices>();
        }
        public ViewGroupModel()
        {

        }
        public ViewGroupModel(IHttpContextAccessor httpContextAccessor, IGroupServices groupServices)
        {

            _httpContextAccessor = httpContextAccessor;
            _groupServices = groupServices;
        }

        internal object GetGroups(DataTablesAjaxRequestModel dataTableAjaxRequestModel)
        {
            var id = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var data = _groupServices.GetGroupsList(
                dataTableAjaxRequestModel.PageIndex,
                dataTableAjaxRequestModel.PageSize,
                dataTableAjaxRequestModel.SearchText,
                 id,
                dataTableAjaxRequestModel.GetSortText(new string[] { "Name" }));


            return new
            {
                recordsTotal = data.total,
                recordsFiltered = data.totalDisplay,
                data = (from record in data.records
                        select new string[]
                        {
                                record.Name.ToString(),
                                record.Id.ToString()

                                
                        }
                    ).ToArray()
            };

        }

    }
}
