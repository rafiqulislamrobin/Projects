using Autofac;
using ClosedXML.Excel;
using DataImporter.Info.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace DataImporter.Areas.User.Models
{
    public class EmailSenderModel
    {
        [Required]
        public string Email { get; set; }
        public int GroupId { get; set; }
        public string FileName { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> Items { get; set; }
        private IDataImporterService _iDataImporterService;
        private ILogger<EmailSenderModel> _logger;
        private ILifetimeScope _scope;
        private IConfiguration configBuilder;
        public EmailSenderModel()
        {
        }

        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;
            _iDataImporterService = _scope.Resolve<IDataImporterService>();
            _logger = _scope.Resolve<ILogger<EmailSenderModel>>();
            configBuilder = _scope.Resolve<IConfiguration>();
        }
        public EmailSenderModel(IDataImporterService iDataImporterService, ILogger<EmailSenderModel> logger
            , IConfiguration ConfigBuilder)
        {
            _iDataImporterService = iDataImporterService;
            _logger = logger;
            configBuilder = ConfigBuilder;
        }


        public void SendEmail(string Email)
        {
            var filepath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "excelfile", $"{FileName}.xlsx"));
            //string filepath = ($"{Directory.GetCurrentDirectory()}{@"\wwwroot\excelfile"}" + "\\" + );
            MailMessage mail = new MailMessage();
            try
            {
                SmtpClient SmtpServer = new SmtpClient(configBuilder.GetValue<string>("Smtp:Host"));
                mail.From = new MailAddress(configBuilder.GetValue<string>("Email:Form"));
                mail.To.Add(Email); // Sending MailTo  
  
                mail.Subject = "Exported User Excel File";
                mail.Body = "Excel File *This is an automatically generated email, please do not reply*";
                System.Net.Mail.Attachment attachment;
                attachment = new Attachment(filepath); //Attaching File to Mail  
                mail.Attachments.Add(attachment);
                SmtpServer.Port = Convert.ToInt32(configBuilder.GetValue<string>("Smtp:Port")); //PORT  
                SmtpServer.EnableSsl =Convert.ToBoolean( configBuilder.GetValue<string>("Email:EnableSsl"));
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(configBuilder.GetValue<string>("Email:Form"), configBuilder.GetValue<string>("Email:Password"));
                SmtpServer.Send(mail);

                if (mail.Attachments != null)
                {
                    for (var i = mail.Attachments.Count - 1; i >= 0; i--)
                    {
                        mail.Attachments[i].Dispose();
                    }
                    mail.Attachments.Clear();
                    mail.Attachments.Dispose();
                }
                mail.Dispose();
                mail = null;
                File.Delete(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "excelfile", $"{FileName}.xlsx")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to sent email");
            }
            
        }


        internal void GetData( int groupId)
        {
           
            var contacts = _iDataImporterService.ContactList(groupId);
            Headers = contacts.Item1;
            Items = contacts.Item2;
            GetExportFiles();
        }
        internal MemoryStream GetExportFiles()
        {

            //start exporting to excel
            var stream = new MemoryStream();

            using (var excelPackage = new ExcelPackage(stream))
            {
                //define a worksheet
                var worksheet = excelPackage.Workbook.Worksheets.Add("Users");

                //putting headers
                for (int i = 1; i <= Headers.Count; i++)
                {
                    var r = 1;
                    worksheet.Cells[r, i].Value = Headers[i - 1];
                    worksheet.Cells[r, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[r, i].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                }

                //putting item in a rows
                for (int row = 0; row < Items.Count; row++)
                {
                    List<string> values = new();
                    for (int col = 0; col < Headers.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = Items[row][col];
                    }
                }
                 excelPackage.Workbook.Properties.Title = "User list";
                 excelPackage.Workbook.Properties.Author = "Robin";

                 FileName = Guid.NewGuid().ToString();
                //saving excel
                var filepath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","excelfile", $"{FileName}.xlsx"));
               
                 excelPackage.SaveAs(new FileInfo (filepath));

            }
            
            return (stream);
        }
    }
}
