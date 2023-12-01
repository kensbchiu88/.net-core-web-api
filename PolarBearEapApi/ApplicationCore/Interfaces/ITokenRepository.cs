using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** Token Repository */
    public interface ITokenRepository
    {
        Task Validate(string id);
        Task<string> Create(string username);
        Task<TokenInfo> GetTokenInfo(string id);

        //預留檢查token時需要檢查是否同一台machine，故把Bind的功能放入Token中
        Task BindMachine(string tokenId, string lineCode, string sectionCode, string stationCode, string serverVersion);
    }
}
