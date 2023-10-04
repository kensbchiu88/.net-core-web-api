namespace PolarBearEapApi.Commons
{
    public interface ITokenService
    {
        void Validate(string tokenString);
        string Create(string username);
    }
}
