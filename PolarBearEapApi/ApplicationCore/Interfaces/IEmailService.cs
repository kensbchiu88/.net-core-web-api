namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IEmailService
    {
        /** Email Service Interface */
        Task Send(string recipients, string subject, string body);
    }
}
