namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IAuthenticationService
    {
        bool Login(string username, string password);
    }
}
