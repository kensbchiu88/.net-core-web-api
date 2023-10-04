using PolarBearEapApi.Models;

namespace PolarBearEapApi.Commons
{
    public class ResponseGenerator
    {
        public static string Ok(string serializeData) {
            return serializeData.Replace("\"OPResponseInfo\":{}", "\"OPResponseInfo\":{\"Result\":\"OK\"}");
        }

        public static string WithOpResponseInfoJson(string serializeData, string returnMessage) {
            return serializeData.Replace("\"OPResponseInfo\":{}", "\"OPResponseInfo\":" + returnMessage);
        }

        public static string Fail(string serializeData)
        {
            return serializeData.Replace("\"OPResponseInfo\":{}", "\"OPResponseInfo\":{\"Result\":\"NG\"}");
        }
    }
}
