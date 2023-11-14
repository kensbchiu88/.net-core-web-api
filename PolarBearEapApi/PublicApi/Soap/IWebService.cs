using System.ServiceModel;


namespace PolarBearEapApi.PublicApi.Soap
{
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        Task<bool> UserLogin(string user, string passwrod);
    }
}
