namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> Login(string username, string password);
    }
}
