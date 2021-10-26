namespace DataImporter.Services
{
    public interface IRecaptchaService
    {
        bool ReCaptchaPassed(string gRecaptchaResponse);
    }
}