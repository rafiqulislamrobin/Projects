
using Autofac;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;


namespace DataImporter.Services
{
    public class EmailService : IEmailService
    {
        private IConfiguration configBuilder;
        private ILifetimeScope _scope;
        public EmailService()
        {

        }
        public void Resolve(ILifetimeScope scope)
        {
            _scope = scope;
            configBuilder = _scope.Resolve<IConfiguration>();
        }
        public EmailService( IConfiguration ConfigBuilder)
        {

            configBuilder = ConfigBuilder;
        }
        public void SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // var configBuilder = new ConfigurationBuilder()
            //.AddJsonFile("appsettings.json", true, true)
            //.Build();

            string fromMail = configBuilder.GetValue<string>("Email:Form");
            string fromPassword = configBuilder.GetValue<string>("Email:Password");

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient(configBuilder.GetValue<string>("Smtp:Host"))
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }

    }
}
