using DataImporter.Info.Business_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Services
{
    public interface IExportServices
    {
        (IList<ExportStatus> records, int total, int totalDisplay) GetExportHistory(int pageIndex, int pageSize,
                         string searchText, string sortText, Guid id, DateTime DateTo, DateTime DateFrom);

        void GetExportStatus(ExportStatus exportStatus);
        (int, DateTime) GetExportHistoryForDownload(int id);
        void SaveExportHistory(ExportStatus exportStatus);
    }
}
