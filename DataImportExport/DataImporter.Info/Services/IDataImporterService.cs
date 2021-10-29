using DataImporter.Info.Business_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Info.Services
{
    public interface IDataImporterService
    {
        void SaveFilePath(FilePath member);
        (IList<FilePath>records, int total, int totalDisplay) GetImporthistory(int pageIndex, int pageSize,
                                    string searchText, string sortText, Guid id , DateTime DateFrom, DateTime DateTo);
 
        string SaveExcelDatatoDb();
        List<ExportStatus> LoadAllExportHistory(Guid id);
        (List<string>, List<string>) ContactList(int groupId);
        (List<string>, List<string>) ContactListByDate(int groupId, 
            DateTime dateFrom, DateTime dateTo);
        List<FilePath> LoadAllImportHistory(Guid id);

        (List<string>, List<string>) ContactListByExportDate(int groupId, DateTime exportDate);
    }
}
