namespace PolarBearEapApi.Commons
{
    public interface ITokenService
    {
        void Validate(string id);
        string Create(string username);
        TokenInfo GetTokenInfo(string id);

        //預留檢查token時需要檢查是否同一台machine，故把Bind的功能放入Token中
        void BindMachine(string tokenId, string lineCode, string sectionCode, string stationCode, string serverVersion);
    }
}
