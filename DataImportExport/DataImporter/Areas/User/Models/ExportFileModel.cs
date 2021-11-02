using Autofac;
using DataImporter.Info.Business_Object;
using DataImporter.Info.Services;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataImporter.Areas.User.Models
{
    public class ExportFileModel
    {
        private  IDataImporterService _iDataImporterService;
        private  IHttpContextAccessor _httpContextAccessor;
        private  IGroupServices _groupServices;
        private  IExportServices _exportServices;
        private ILifetimeScope _scope;

        public List<int> GroupIds { get; set; }
        public int GroupId { get; set; }
        public DateTime ExportDate{ get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<string> Headers { get; set; }
        public List<string> Itemss { get; set; }
        public List<List<string>> Items { get; set; }

      
        public ExportFileModel()
        { 

        }
        public void Resolve(ILifetimeScope scope)
        {
            _iDataImporterService =_scope.Resolve<IDataImporterService>();
            _httpContextAccessor = _scope.Resolve<IHttpContextAccessor>();
            _groupServices = _scope.Resolve<IGroupServices>();
            _exportServices = _scope.Resolve<IExportServices>();
            _scope = scope;
        }

        public ExportFileModel(IDataImporterService iDataImporterService , 
            IHttpContextAccessor httpContextAccessor, IGroupServices groupServices, IExportServices exportServices)
        {
            _iDataImporterService = iDataImporterService;
            _httpContextAccessor = httpContextAccessor;
            _groupServices = groupServices;
            _exportServices = exportServices;

        }

        internal void GetContactsList(int groupId)
        {
            if (groupId>0)
            {
                var contacts = _iDataImporterService.ContactListByDate(groupId, DateFrom, DateTo);
                Headers = contacts.Item1;
                Itemss = contacts.Item2;
                GroupId = groupId;
            }
         
        }
        internal void GetContactsList(int groupId , DateTime datefrom ,DateTime dateto)
        {
            if (groupId > 0)
            {
                var contacts = _iDataImporterService.ContactListByDate(groupId, datefrom, dateto);
                Headers = contacts.Item1;
                Itemss = contacts.Item2;
                GroupId = groupId;
            }

        }
        internal void GetContactsListByDate(int groupId)
        {
          
                var contacts = _iDataImporterService.ContactListByExportDate(groupId , ExportDate);
                Headers = contacts.Item1;
                Itemss = contacts.Item2;
                GroupId = groupId;
            
        }
        internal MemoryStream GetExportFile()
        {

            //start exporting to excel
            var stream = new MemoryStream();
            
            using (var excelPackage = new ExcelPackage(stream))
            {
                //define a worksheet
                var worksheet = excelPackage.Workbook.Worksheets.Add("Users");

                for (int i = 1; i <= Headers.Count; i++)
                {
                    var r = 1;
                    worksheet.Cells[r, i].Value = Headers[i-1];
                    worksheet.Cells[r, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[r, i].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                }

                //filling information
                 int col = 0,row = 0,head = 0,z = 0;
              
                for (; col < Itemss.Count; col++,head++,z++)
                {
                    if (head == Headers.Count)
                    {
                        z = 0;
                        head = 0;
                        row++;
                    }
                    worksheet.Cells[row + 2, z + 1].Value = Itemss[col];
                   
                }
                excelPackage.Workbook.Properties.Title = "User list";
                excelPackage.Workbook.Properties.Author = "Robin";
                excelPackage.Save();

            }
            stream.Position = 0;
            return(stream);
        }

        internal void GetExportFileHistory(int id)
        {
            var items = _exportServices.GetExportHistoryForDownload(id);
            GroupId = items.Item1;
            ExportDate = items.Item2;
        }
        internal MemoryStream GetExportMultipleFiles(List<int> id)
        {


            //start exporting to excel
            var stream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(stream))
            {
                foreach (var groupid in id)
                {


                    var contacts = _iDataImporterService.ContactList(groupid);
                    Headers = new();
                    Items = new();
                    Headers = contacts.Item1;
                    Itemss = contacts.Item2;
                    GroupId = groupid;
                    var group = _groupServices.LoadGroup(groupid);
                    var worksheet = excelPackage.Workbook.Worksheets.Add($"{group.Name}");

                    for (int i = 1; i <= Headers.Count; i++)
                    {
                        var r = 1;
                        worksheet.Cells[r, i].Value = Headers[i - 1];
                        worksheet.Cells[r, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[r, i].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    }

                    //filling information
                    int col = 0, row = 0, head = 0, z = 0;

                    for (; col < Itemss.Count; col++, head++, z++)
                    {
                        if (head == Headers.Count)
                        {
                            z = 0;
                            head = 0;
                            row++;
                        }
                        worksheet.Cells[row + 2, z + 1].Value = Itemss[col];

                    }

                    excelPackage.Workbook.Properties.Title = "User list";
                    excelPackage.Workbook.Properties.Author = "Robin";


                }
                //define a worksheet

                excelPackage.Save();
            }

            return (stream);
        }


        internal List<Group> LoadGroupsWithContactAvailable()
        {
            var id = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _groupServices.LoadGroupsWithContact(id);
        }


      
    }
}

