using System.ServiceModel;


namespace PolarBearEapApi.PublicApi.Soap
{
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        bool UserLogin(string user, string passwrod);
    }
}
