using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace PolarBearEapApi.ApplicationCore.Models
{
    /** 對應IMesService 回傳值的 Response Model */
    public class FITMesResponse
    {
        [JsonProperty("Result", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? Result { get; set; }
        [JsonProperty("ResultCoded", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? ResultCode { get; set; }
        [JsonProperty("MessageCode", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? MessageCode { get; set; }
        [JsonProperty("Display", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? Display { get; set; }
        [JsonProperty("BindInfo", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? BindInfo { get; set; }

        public bool IsResultOk()
        {
            if (string.IsNullOrEmpty(Result) || !Result.ToUpper().Equals("OK"))
            {
                return false;
            }
            return true;
        }

        /** 檢查MES執行結果 */
        public static bool IsResultOk(string mesReturnJson)
        {
            if (string.IsNullOrWhiteSpace(mesReturnJson)) return false;

            FITMesResponse? fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturnJson);
            return fitMesResponse.IsResultOk();
        }

        /** 將呼叫StoredProces */
        public static FITMesResponse ConvertFromStoredProcedureResult(string storedProcedureResult)
        {
            FITMesResponse response = new FITMesResponse();

            if (storedProcedureResult.IsNullOrEmpty())
            {
                response.Result = "NG";
                response.Display = "No Data Return";
            }
            else
            {
                var results = storedProcedureResult.Split(',').ToList();
                if (results.Count != 3)
                {
                    response.Result = "NG";
                    response.Display = "Invalid Mes Return Format";
                }
                else
                {
                    if ("OK".Equals(results[0], StringComparison.OrdinalIgnoreCase))
                    {
                        response.Result = "OK";
                        response.ResultCode = results[2];
                    }
                    else
                    {
                        response.Result = "NG";
                        response.Display = results[1];
                    }
                }
            }

            return response;
        }
    }
}
