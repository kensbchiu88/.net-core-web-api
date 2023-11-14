namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IEmailService
    {
        Task Send(string recipients, string subject, string body);
    }
}
