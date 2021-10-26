using Autofac;
using DataImporter.Common.Utility;
using DataImporter.Info.Business_Object;
using DataImporter.Info.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataImporter.Areas.User.Models
{
    public class ExportStatusModel
    {

        public string DownloadStatus { get; set; }
        public string EmailStatus { get; set; }
        public DateTime Date { get; set; }
        public int GroupId { get; set; }
        public int Id { get; set; }

        public IDatetimeUtility _dateTimeUtility;
        private IExportServices _exportServices;
        private ILifetimeScope _scope;
        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;
            _dateTimeUtility = _scope.Resolve<IDatetimeUtility>();
            _exportServices = _scope.Resolve<IExportServices>();
        }
        public ExportStatusModel()
        {

        }
        public ExportStatusModel( IDatetimeUtility dateTimeUtility, IExportServices exportServices)
        {

            _dateTimeUtility = dateTimeUtility;
            _exportServices = exportServices;
        }
        internal void MakeStatus(int groupId , string email)
        {

            
                    ExportStatus exportStatus = new();
                    exportStatus.Email = email;                  
                    exportStatus.DateTime = _dateTimeUtility.Now;
                    exportStatus.GroupId = groupId;
                    _exportServices.SaveExportHistory(exportStatus);

        }
    }
}
