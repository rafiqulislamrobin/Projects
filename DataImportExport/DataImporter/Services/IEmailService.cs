namespace DataImporter.Services
{
    public interface IEmailService 
    {
        void SendEmailAsync(string email, string subject, string htmlMessage);
    }
}