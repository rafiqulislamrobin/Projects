using DataImporter.Common.Utility;
using DataImporter.Info.Business_Object;
using DataImporter.Info.Exceptions;
using DataImporter.Info.UnitOfWorks;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DataImporter.Info.Services
{
    public class DataImporterService : IDataImporterService
    {
        private readonly IDataUnitOfWork _dataUnitOfWork;
        private readonly IDatetimeUtility _DatetimeUtility;
        public DataImporterService(IDataUnitOfWork dataUnitOfWork , IDatetimeUtility DatetimeUtility)
        {
            _dataUnitOfWork = dataUnitOfWork;
            _DatetimeUtility = DatetimeUtility;
        }

        public (List<string>, List<string>) ContactList(int groupId)
        {
            //var group = _dataUnitOfWork.Group.GetById(groupId);

            List<string> headers = new();
            //List<string> items = new();
            List<string> itemsRow = new();

            var contactEntities = _dataUnitOfWork.Contact.GetAll().Where(x => x.GroupId == groupId);
           
            foreach (var contactRow in contactEntities)
            {
                if (headers.Contains(contactRow.Key))
                {
                    break;
                }
                else
                {
                    headers.Add(contactRow.Key);
                }

            }
            itemsRow = (from contacts in contactEntities
                        select contacts.Value).ToList();

            return (headers, itemsRow);
        }
        public (List<string>, List<string>) ContactListByExportDate(int groupId, DateTime dateTime)
        {
            //var group = _dataUnitOfWork.Group.GetById(groupId);

            List<string> headers = new();
           
            List<string> itemsRow = new();

            var contactEntities = _dataUnitOfWork.Contact.GetAll().Where(x => x.GroupId == groupId && x.ContactDate <= dateTime);
            var h = 0;
            foreach (var contactRow in contactEntities)
            {
                if (headers.Contains(contactRow.Key))
                {
                    break;
                }
                else
                {
                    headers.Add(contactRow.Key);
                }
            }
            itemsRow = (from contacts in contactEntities
                        select contacts.Value).ToList();

            return (headers, itemsRow);
        }




        public (List<string>, List<string>) ContactListByDate(int groupId,
                DateTime DateFrom, DateTime DateTo)
        {

            List<string> headers = new();
            List<string> items = new();
            //List<List<string>> itemsRow = new();
            List<string> itemsRow = new();

            if (DateTo != DateTime.MinValue && DateFrom != DateTime.MinValue)
            {
                var contactEntities = _dataUnitOfWork.Contact.GetAll().Where(x => x.GroupId == groupId);
                var contactsByDate = from contacts in contactEntities
                                     where contacts.ContactDate <= DateTo && contacts.ContactDate >= DateFrom
                                     select contacts;
                var j = from contacts in contactEntities
                        where contacts.ContactDate <= DateTo && contacts.ContactDate >= DateFrom
                        select contacts.Key;

        
                foreach (var contactRow in contactsByDate)
                {
                    if (headers.Contains(contactRow.Key))
                    {
                        break;
                    }
                    else
                    {
                        headers.Add(contactRow.Key);
                    }
                }
                //foreach (var contactRow in contactsByDate)
                //{
                //    items.Add(contactRow.Value);
                //    h++;
                //    if (h == headers.Count)
                //    {
                //        itemsRow.Add(items);
                //        items = new List<string>();
                //        h = 0;
                //    }
                //}
            }
            else
            {
                var contactEntities = _dataUnitOfWork.Contact.GetAll().Where(x => x.GroupId == groupId );

                itemsRow = (from contacts in contactEntities
                        select contacts.Value).ToList();
                
                foreach (var contactRow in contactEntities)
                {
                    if (headers.Contains(contactRow.Key))
                    {
                        break;
                    }
                    else
                    {
                        headers.Add(contactRow.Key);
                    }
                }
                //foreach (var contactRow in contactEntities)
                //{
                //    items.Add(contactRow.Value);
                //    h++;
                //    if (h == headers.Count)
                //    {
                //        itemsRow.Add(items);
                //        items = new List<string>();
                //        h = 0;
                //    }
                //}
            }
        
            return (headers, itemsRow);
        }





        public (IList<FilePath> records, int total, int totalDisplay) GetImporthistory(int pageIndex, int pageSize, string searchText, string sortText,
                                 Guid id, DateTime DateFrom, DateTime DateTo)
        {
            if (DateFrom != DateTime.MinValue && DateTo != DateTime.MinValue)
            {
                        var historyData = _dataUnitOfWork.FilePath.GetDynamic(
                        string.IsNullOrWhiteSpace(searchText) ? x => x.Group.ApplicationUserId == id : x => x.FileName.Contains(searchText) && x.Group.ApplicationUserId == id,
                           sortText, "Group", pageIndex, pageSize, true);
                        var datas = historyData.data;

                        var resultHistory = (from history in historyData.data
                                             where history.DateTime >= DateFrom && history.DateTime <= DateTo
                                             select new FilePath
                                             {
                                                 GroupName = history.Group.Name,
                                                 FileStatus = history.FileStatus,
                                                 FileName = history.FileName,
                                                 FilePathName = history.FilePathName,
                                                 DateTime = history.DateTime
                                             }).ToList();

                return (resultHistory, historyData.total, historyData.totalDisplay);
            }
            else
            {
                        var historyData = _dataUnitOfWork.FilePath.GetDynamic(
           string.IsNullOrWhiteSpace(searchText) ? x => x.Group.ApplicationUserId == id : x => x.FileName.Contains(searchText) && x.Group.ApplicationUserId == id,
              sortText, "Group", pageIndex, pageSize, true);
                        var datas = historyData.data;

                var resultHistory = (from history in historyData.data
                                     
                                     select new FilePath
                                     {
                                         GroupName = history.Group.Name,
                                         FileStatus = history.FileStatus,
                                         FileName = history.FileName,
                                         FilePathName = history.FilePathName,
                                         DateTime = history.DateTime
                                     }).ToList();
                return (resultHistory, historyData.total, historyData.totalDisplay);
            }

    
        }

        public List<ExportStatus> LoadAllExportHistory(Guid id)
        {
            var ExportStatusEntities = _dataUnitOfWork.ExportStatus.GetAll().Where(g => g.Group.ApplicationUserId == id);
            
            var result = (from e in ExportStatusEntities

                          select new ExportStatus
                          {
                              Id = e.Id,
                              Email = e.Email,
                              GroupName = e.Group.Name,
                              DateTime =e.DateTime,

                          }).ToList();
            return result;
        }

      
        public List<FilePath> LoadAllImportHistory(Guid id)
        {
            var ExportStatusEntities = _dataUnitOfWork.FilePath.Get(x=>x.Group.ApplicationUserId==id,"Group");

            var result = (from e in ExportStatusEntities
                          where e.Group.ApplicationUserId ==id
                          select new FilePath
                          {
                              Id = e.Id,
                              FilePathName = e.FilePathName,
                              GroupName = e.GroupName,
                              DateTime = e.DateTime,
                              FileStatus =e.FileStatus,
                              FileName =e.FileName,
                              GroupId =e.GroupId

                          }).ToList();
            return result;
        }

        public string SaveExcelDatatoDb()
        {

            var fileEntities = _dataUnitOfWork.FilePath.GetAll();
            string fileroot = "";
            string fileStatus = "";
            int fileId = 0;
            int GroupId = 0;
            foreach (var f in fileEntities)
            {
                if (f.FileStatus.ToLower() == "pending")
                {
                    fileroot = f.FilePathName;
                    fileId = f.Id;
                    GroupId = f.GroupId;
                    fileStatus = f.FileStatus = "processing";
                    _dataUnitOfWork.Save();
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (var stream = System.IO.File.Open(fileroot, FileMode.Open, FileAccess.Read))
                    {


                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            Contact contact = new();
                            Dictionary<int, Dictionary<string, string>> cont = new();
                            List<string> headers = new();

                            var conf = new ExcelDataSetConfiguration
                            {
                                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                                {
                                    UseHeaderRow = false
                                }
                            };
                            var dataSet = reader.AsDataSet(conf);

                            var dataTable = dataSet.Tables[0];

                            for (var i = 0; i < 1; i++)
                            {
                                for (var j = 0; j < dataTable.Columns.Count; j++)
                                {
                                    headers.Add(dataTable.Rows[i][j].ToString());

                                }

                            }
                            for (var i = 1; i < dataTable.Rows.Count; i++)
                            {
                                Contact contacts = new();
                                for (var j = 0; j < dataTable.Columns.Count; j++)
                                {
                                    var z = dataTable.Rows[i][j].ToString();
                                    if (z != null && z != "")
                                    {
                                        contacts.Properties.Add(headers[j], dataTable.Rows[i][j].ToString());
                                    }
                                    else
                                    {
                                        contacts.Properties.Add(headers[j], "");
                                    }
                                }
                                cont.Add(i + 1, contacts.Properties);

                            }
                            foreach (var item in cont)
                            {
                                var value = item.Value;

                                foreach (var v in value)
                                {
                                    _dataUnitOfWork.Contact.Add(new Entities.Contact
                                    {
                                        ExcelRow = item.Key,
                                        Key = v.Key,
                                        Value = v.Value,
                                        GroupId = GroupId,
                                        ContactDate = _DatetimeUtility.Now
                                    });
                                }
                            }
                            
                        }
                    }


                    var file = _dataUnitOfWork.FilePath.GetById(fileId);
                    file.FileStatus = "Completed";
                    _dataUnitOfWork.Save();
                    try
                    {
                        File.Delete(fileroot);
                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                    return "Deleted ";

                }
            }
            return "no file to delete";
        }


        public void SaveFilePath(FilePath filepath)
        {


            _dataUnitOfWork.FilePath.Add(
                new Entities.FilePath
                {
                    FileName = filepath.FileName,
                    FilePathName = filepath.FilePathName,
                    DateTime = filepath.DateTime,
                    GroupId = filepath.GroupId,
                    GroupName = filepath.GroupName,
                    FileStatus = filepath.FileStatus



                });
            _dataUnitOfWork.Save();
        }




    }
}
