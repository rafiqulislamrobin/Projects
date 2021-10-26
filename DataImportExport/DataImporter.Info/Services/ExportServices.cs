using DataImporter.Info.Business_Object;
using DataImporter.Info.Exceptions;
using DataImporter.Info.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Services
{
    public class ExportServices : IExportServices
    {
        private readonly IDataUnitOfWork _dataUnitOfWork;


        public ExportServices(IDataUnitOfWork dataUnitOfWork)
        {
            _dataUnitOfWork = dataUnitOfWork;

        }

        public void GetExportStatus(ExportStatus exportStatus)
        {
            var exportEntity = _dataUnitOfWork.ExportStatus.GetById(exportStatus.Id);
            if (exportEntity != null)
            {
                exportEntity.Id = exportStatus.Id;

                exportEntity.Email = exportStatus.Email;
                exportEntity.DateTime = exportStatus.DateTime;
                _dataUnitOfWork.Save();
            }
            else
                throw new InvalidOperationException("History is not available");
        }

        public (IList<ExportStatus> records, int total, int totalDisplay) GetExportHistory(int pageIndex, int pageSize,
              string searchText, string sortText, Guid id, DateTime dateTo, DateTime DateFrom)
        {
            if (DateFrom != DateTime.MinValue && dateTo != DateTime.MinValue)
            {
                var historyData = _dataUnitOfWork.ExportStatus.GetDynamic(
           string.IsNullOrWhiteSpace(searchText) ? x => x.Group.ApplicationUserId == id : x => x.Email.Contains(searchText) && x.Group.ApplicationUserId == id,
          sortText, "Group", pageIndex, pageSize, true);

                var resultHistory = (from history in historyData.data
                                     where history.DateTime >= DateFrom && history.DateTime <= dateTo
                                     select new ExportStatus
                                     {
                                         Id = history.Id,
                                         DateTime = history.DateTime,
                                         GroupName = history.Group.Name,
                                         Email = history.Email,

                                     }).ToList();

                return (resultHistory, historyData.total, historyData.totalDisplay);
            }
            else
            {
                var historyData = _dataUnitOfWork.ExportStatus.GetDynamic(
           string.IsNullOrWhiteSpace(searchText) ? x => x.Group.ApplicationUserId == id : x => x.Email.Contains(searchText) && x.Group.ApplicationUserId == id,
          sortText, "Group", pageIndex, pageSize, true);

                var resultHistory = (from history in historyData.data
                                     select new ExportStatus
                                     {
                                         Id = history.Id,
                                         DateTime = history.DateTime,
                                         GroupName = history.Group.Name,
                                         Email = history.Email,

                                     }).ToList();

                return (resultHistory, historyData.total, historyData.totalDisplay);
            }
        }



        public (int, DateTime) GetExportHistoryForDownload(int id)
        {
            var exportStatus = _dataUnitOfWork.ExportStatus.GetById(id);
            var groupId = exportStatus.GroupId;
            var dateTime = exportStatus.DateTime;
            return (groupId, dateTime);
        }

        public void SaveExportHistory(ExportStatus exportStatus)
        {
            if (exportStatus!=null)
            {
                _dataUnitOfWork.ExportStatus.Add(
                     new Entities.ExportStatus
                     {
                         Id = exportStatus.Id,
                         Email = exportStatus.Email,
                         DateTime = exportStatus.DateTime,
                         GroupId = exportStatus.GroupId
                     });
                _dataUnitOfWork.Save();
            }
            else
            {
                throw new InvalidOperationException("export status is not available");
            }
        }
    }
}
