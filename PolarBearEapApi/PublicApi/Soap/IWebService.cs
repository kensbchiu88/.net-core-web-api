using System.Diagnostics.Metrics;
using System.ServiceModel;
using static FIT.MES.Service.CommonEnum;


namespace PolarBearEapApi.PublicApi.Soap
{
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        Task<bool> UserLogin(string user, string passwrod);
        [OperationContract]
        List<List<string>> GetLineList();
        [OperationContract]
        Task<string> GetStationByLineSection(string lineCode, string sectionCode);
        [OperationContract]
        Task<bool> CheckDevRoute(string sn, string sectionCode, string sectionDesc, string stationCode, string StationDesc, string lineCode, string tester);
        [OperationContract]
        Task<bool> CommitDevData(string sn, string sectionCode, string sectionDesc, string stationCode, string stationDesc, string lineCode, string tester, string test_time, int testResult, string testData);
    }
}
