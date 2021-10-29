using AspNetCoreHero.ToastNotification.Abstractions;
using Autofac;
using DataImporter.Areas.User.Models;
using DataImporter.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;


namespace DataImporter.Areas.User.Controllers
{
    [Area("User"), Authorize(Policy = "RestrictedArea")]
    public class DataImporterController : Controller
    {
        private readonly ILogger<DataImporterController> _logger;
        private readonly ILifetimeScope _scope;
  
        private INotyfService _notyfService;
        public int temp { get; set; }
        public DataImporterController(ILogger<DataImporterController> logger , ILifetimeScope scope,
            INotyfService notyfService)
        {
            _logger = logger;
            _scope = scope;
            _notyfService = notyfService;
        }
       
        public IActionResult Index()
        {
            var model = _scope.Resolve<IndexModel>();
            //var model = new IndexModel();
            model.GetTotal();
            return View(model);
        }
        
        public IActionResult ViewGroups()
        {
            var model = _scope.Resolve<ViewGroupModel>();
            return View(model);
        }
        public JsonResult GetGroupsData()
        {
            var dataTableAjaxRequestModel = new DataTablesAjaxRequestModel(Request);
            var model = _scope.Resolve<ViewGroupModel>();
            var data = model.GetGroups(dataTableAjaxRequestModel);
            return Json(data);
        }
        public IActionResult CreateGroups()
        {
            var model = _scope.Resolve<CreateGroupModel>(); ;
            return View(model);
        }
        [HttpPost]
        public IActionResult CreateGroups(CreateGroupModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    model.CreateGroup();
                    _notyfService.Custom("Group Create Successfully.", 4, "#f5f5f5", "fas fa-check-circle");
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", "Failed to Create Group");
                    if (ex.Message == "Group name is already used")
                    {
                        _notyfService.Custom("Group name is already used.", 4, "#c92f04", "fas fa-times-circle");
                    }
                    _logger.LogError(ex, "Add Group Failed");
                }

            }
            return View(model);
        }
        public IActionResult EditGroup(int id)
        {
            var model = _scope.Resolve<EditGroupModel>();
            model.LoadModelData(id);
            return View(model);

        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public IActionResult EditGroup(EditGroupModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Resolve(_scope);
                    model.Update();
                    _notyfService.Custom("Group edited Successfully.", 4, "#f5f5f5", "fas fa-check-circle");
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", "Failed to Create Group");
                    if (ex.Message == "Group name is already used")
                    {
                        _notyfService.Custom(ex.Message, 4, "#c92f04", "fas fa-times-circle");
                    }
                    _logger.LogError(ex, "Upload Group Failed");
                    return View(model);
                }
            }
            return View(nameof(ViewGroups));
        }
        public IActionResult DeleteGroup(int id)
        {
            var model = _scope.Resolve <CreateGroupModel>();
            model.DeleteGroup(id);
            _notyfService.Custom("Group Deleted Successfully.", 4, "#f5f5f5", "fas fa-check-circle");
            return RedirectToAction(nameof(ViewGroups));
        }




        public IActionResult ImportFile()
        {
            var model = _scope.Resolve <FilePathModel>();
            var list = model.LoadAllGroups();
            ViewBag.GroupList = new SelectList(list, "Id", "Name");
            return View(model);

        }
        [HttpPost]
        public IActionResult ImportFile(IFormFile file, FilePathModel filepathmodel)
        {
            var model = _scope.Resolve<FilePathModel>();
            model.Resolve(_scope);
            model.GroupId = filepathmodel.GroupId;

            //convert to a stream
            var filepath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "excelfile", file.FileName));
            //var filepath = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\excelfile"}" + "\\" + file.FileName;
            using (FileStream fileStream = System.IO.File.Create(filepath))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            var path = Path.GetFileName(filepath);
            model.file = path;
            return RedirectToAction("ConfirmContacts", model);

        }
        public IActionResult ConfirmContacts(FilePathModel filemodels)
        {
            ConfirmFile model = _scope.Resolve<ConfirmFile>();
            model.Resolve(_scope);
            filemodels.Resolve(_scope);
            model.GroupId = filemodels.GroupId;
            model.file = filemodels.file;
            var file = filemodels.file;
            var filepath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "excelfile", file));
            //var filepath = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\excelfile"}" + "\\" + file;

            var fileStatus = filemodels.GetGroupStatusById(filemodels.GroupId);

            var list = filemodels.LoadAllGroups();

                if (fileStatus.Item1 == "incomplete")
                {
                    if (fileStatus.Item2 == file)
                    {
                        ViewBag.FileProcess = "A file is processing in this group , please wait  ";
                        ViewBag.GroupList = new SelectList(list, "Id", "Name");
                        return View(nameof(ImportFile));
                    }
                    else
                    {
                        ViewBag.FileProcess = "A file is processing in this group , please wait  ";
                        System.IO.File.Delete(filepath);
                        ViewBag.GroupList = new SelectList(list, "Id", "Name");
                        return View(nameof(ImportFile));
                    }

                }



            var z = model.ConfirmFileUpload(filepath);
            if (z.Item2==null)
            {
                ViewBag.HeaderMissMatch = "FIles Columns dosent match to this group." +
                                          " Please select another group or create a group";
                System.IO.File.Delete(filepath);
                ViewBag.GroupList = new SelectList(list, "Id", "Name");
                return View(nameof(ImportFile));
            }
            return View(model);

        }
        [HttpPost]
        public IActionResult ConfirmContacts(ConfirmFile model)
        {

            var FilePathModels = _scope.Resolve<FilePathModel>();
           
            model.Resolve(_scope);
            var list = FilePathModels.LoadAllGroups();

            FilePathModels.SaveFilePath(model.file, model.GroupId, list);
            return RedirectToAction(nameof(ImportHistory));

        }
        public IActionResult CancelImportFile(ConfirmFile ConfirmModel)
        {
            var model = _scope.Resolve<FilePathModel>();
            ConfirmModel.Resolve(_scope);
            var list = model.LoadAllGroups();
            ViewBag.GroupList = new SelectList(list, "Id", "Name");
            model.CancelImport(ConfirmModel.file);
            return View(nameof(ImportFile));
        }


        public IActionResult ImportHistory(ImportHistoryModel importHistoryModel)
        {
            importHistoryModel.Resolve(_scope);
            TempData["DateTo"] = importHistoryModel.DateTo;
            TempData["DateFrom"] = importHistoryModel.DateFrom;
            return View();
           

        }
        public JsonResult GetImportHistoryData()
        {
            
            var dataTableAjaxRequestModel = new DataTablesAjaxRequestModel(Request);
            var model =_scope.Resolve<ImportHistoryModel>();
            model.DateTo = Convert.ToDateTime(TempData["DateTo"]);
            model.DateFrom = Convert.ToDateTime(TempData["DateFrom"]);
            var data = model.GetHistories(dataTableAjaxRequestModel);
            return Json(data);
        }
       
        public IActionResult ViewContacts()
        {
            var model = _scope.Resolve<ExportFileModel>();
            var list = model.LoadAllGroups();
            ViewBag.GroupList = new SelectList(list, "Id", "Name");

            return View(model);
        }
        [HttpPost]
        public IActionResult ViewContacts(ExportFileModel exportFileModel)
        {
 
            var model = _scope.Resolve<ExportFileModel>();
            model.GetContactsList(exportFileModel.GroupId, exportFileModel.DateFrom,exportFileModel.DateTo);

            var list = model.LoadAllGroups();
            if (model.Headers.Count == 0)
            {
                _notyfService.Error("No Contact Available", 10);
                ViewBag.ContactListNullMassage = "(No Contact Available)";
                ViewBag.GroupList = new SelectList(list, "Id", "Name");
            }
            else
            {
                ViewBag.GroupList = new SelectList(list, "Id", "Name");
            }

            TempData["id"] = exportFileModel.GroupId;

            return View(model);
        }
        [HttpGet]
        public IActionResult ExportFile()
        {


            var model = _scope.Resolve<ExportFileModel>();
            var list = model.LoadAllGroups();
            ViewBag.GroupList = new SelectList(list, "Id", "Name");
        
            return View(model);
            
        }
        [HttpPost]
        public IActionResult ExportFile(FilePathModel filePathmodel)
        {
            filePathmodel.Resolve(_scope);
            var model = _scope.Resolve<ExportFileModel>();
            model.GetContactsList(filePathmodel.GroupId);
            
            var list = model.LoadAllGroups();
            if (model.Headers.Count == 0)
            {
                _notyfService.Error("No Contact Available", 4);
                ViewBag.ContactListNullMassage = "(No Contact Available)";
                ViewBag.GroupList = new SelectList(list, "Id", "Name");
            }
            else
            { 
                ViewBag.GroupList = new SelectList(list, "Id", "Name");
            }

            List<string> headers = new();
            foreach (var item in model.Headers)
            {
                headers.Add(item);
            }

            TempData["id"] = filePathmodel.GroupId;

            return View(model);

        }
      
        public IActionResult Download()
        {
            var id = Convert.ToInt32(TempData.Peek("id"));

            var model = _scope.Resolve<ExportFileModel>();
            model.GetContactsList(id);
            var contacts = model.GetExportFile();
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileFormat = "User.xlsx";
            return File(contacts, fileType, fileFormat);
        }
        public IActionResult DownloadFromExportHistory(int id)
        {

            var model = _scope.Resolve<ExportFileModel>();
            model.GetExportFileHistory(id);
            model.GetContactsListByDate(model.GroupId) ;
            var contacts = model.GetExportFile();
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileFormat = "User.xlsx";
            return File(contacts, fileType, fileFormat);
        }
        public IActionResult EmailSender()
        {
            var model = _scope.Resolve<EmailSenderModel>();
            model.GroupId = Convert.ToInt32(TempData.Peek("id"));
            return View(model);
        }
        [HttpPost]
        public IActionResult EmailSender(EmailSenderModel emailSenderModel)
        {
            emailSenderModel.Resolve(_scope);
            var groupId = emailSenderModel.GroupId;
            var email = (emailSenderModel.Email);

            emailSenderModel.GetData(groupId);
            emailSenderModel.SendEmail(email);

            var model = _scope.Resolve<ExportStatusModel>();
            model.MakeStatus(groupId, email);
            _notyfService.Custom("Email sent", 4, "#f5f5f5", "fas fa-check-circle");
            return RedirectToAction(nameof(ExportFileHistory));
        }

        public IActionResult ExportFileHistory(ExportHistoryModel model )
        {
            model.Resolve(_scope);
            TempData["DateTo"] = model.DateTo;
            TempData["DateFrom"] = model.DateFrom;
            return View();
        }
        public JsonResult GetExporttHistoryData()
        {
            var dataTableAjaxRequestModel = new DataTablesAjaxRequestModel(Request);
            var model = _scope.Resolve<ExportHistoryModel>();
            model.DateTo = Convert.ToDateTime(TempData["DateTo"]);
            model.DateFrom = Convert.ToDateTime(TempData["DateFrom"]);
            var data = model.GetHistories(dataTableAjaxRequestModel);
            return Json(data);
        }
        [HttpGet]
        public IActionResult ExportFileMultiple()
        {

            var model = _scope.Resolve<ExportFileModel>();
            var list = model.LoadAllGroups();
            ViewBag.GroupList = new SelectList(list, "Id", "Name");

            return View(model);

        }
        [HttpPost]
        public IActionResult ExportFileMultiple(FilePathModel filePathmodel)
        {
            filePathmodel.Resolve(_scope);
            var model = _scope.Resolve<ExportFileModel>();
            //model.GetContactsList(filePathmodel.GroupId);

            var list = model.LoadAllGroups();

            ViewBag.GroupList = new SelectList(list, "Id", "Name");

            TempData["GroupIds"] = filePathmodel.GroupIds;
            return RedirectToAction("DownloadMultiple", filePathmodel);

        }
        public IActionResult DownloadMultiple(FilePathModel filePathmodel)
        {

            var model = _scope.Resolve<ExportFileModel>();
            var contacts = model.GetExportMultipleFiles(filePathmodel.GroupIds);
            
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileFormat = "User.xlsx";
            contacts.Position = 0;
            return File(contacts, fileType, fileFormat);

        }

    }
}
