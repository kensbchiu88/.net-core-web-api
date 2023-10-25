﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PolarBearEapApi.ApplicationCore.Extensions
{
    public class ResponseSerializeDataGenerator
    {
        public static string Ok(string serializeData)
        {
            //為了讓Request與Response盡量相同，使用string.Replace方式取代OPResponseInfo
            if (serializeData.Contains("OPResponseInfo"))
                return serializeData.Replace("\"OPResponseInfo\":{}", "\"OPResponseInfo\":{\"Result\":\"OK\"}");

            //如果Request沒有帶OPResponseInfo資訊，使用string.Replace()會讓Response也沒有OPResponseInfo，需重新組合Json
            //重組的Json可能會與Request有點不同。例如 StationCode: 重組後會變成 StationCode: null
            string lineCode = JsonUtil.GetParameter(serializeData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializeData, "SectionCode");
            int? stationCode = string.IsNullOrEmpty(JsonUtil.GetParameter(serializeData, "StationCode")) ? null : int.Parse(JsonUtil.GetParameter(serializeData, "StationCode")!);
            string opCategory = JsonUtil.GetParameter(serializeData, "OPCategory");
            string opRequestInfo = JsonUtil.GetParameter(serializeData, "OPRequestInfo");
            var newOpResponseInfo = "{\"Result\":\"OK\"}";
            return GenerateSerializeData(lineCode, sectionCode, stationCode, opCategory, opRequestInfo, newOpResponseInfo);
        }

        public static string WithOpResponseInfoJson(string serializeData, string returnMessage)
        {
            //為了讓Request與Response盡量相同，使用string.Replace方式取代OPResponseInfo
            if (serializeData.Contains("OPResponseInfo"))
                return serializeData.Replace("\"OPResponseInfo\":{}", "\"OPResponseInfo\":" + returnMessage);

            //如果Request沒有帶OPResponseInfo資訊，使用string.Replace()會讓Response也沒有OPResponseInfo，需重新組合Json
            //重組的Json可能會與Request有點不同。例如 StationCode: 重組後會變成 StationCode: null
            string lineCode = JsonUtil.GetParameter(serializeData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializeData, "SectionCode");
            int? stationCode = string.IsNullOrEmpty(JsonUtil.GetParameter(serializeData, "StationCode")) ? null : int.Parse(JsonUtil.GetParameter(serializeData, "StationCode")!);
            string opCategory = JsonUtil.GetParameter(serializeData, "OPCategory");
            string opRequestInfo = JsonUtil.GetParameter(serializeData, "OPRequestInfo");
            var newOpResponseInfo = returnMessage;
            return GenerateSerializeData(lineCode, sectionCode, stationCode, opCategory, opRequestInfo, newOpResponseInfo);
        }

        public static string Fail(string serializeData) 
        {
            string lineCode = JsonUtil.GetParameter(serializeData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializeData, "SectionCode");
            int? stationCode = string.IsNullOrEmpty(JsonUtil.GetParameter(serializeData, "StationCode")) ? null : int.Parse(JsonUtil.GetParameter(serializeData, "StationCode")!);
            string opCategory = JsonUtil.GetParameter(serializeData, "OPCategory");
            string opRequestInfo = JsonUtil.GetParameter(serializeData, "OPRequestInfo");
            string opResponseInfo = JsonUtil.GetParameter(serializeData, "OPResponseInfo");

            var newOpResponseInfo = GetFailOpResponseInfo(opCategory, opResponseInfo);
            return GenerateSerializeData(lineCode, sectionCode, stationCode, opCategory, opRequestInfo, newOpResponseInfo);
        }

        private static string GenerateSerializeData(string lineCode, string sectionCode, int? stationCode, string opCategory, string opRequestInfo, string opResponseInfo)
        { 
            var result = new JObject(); 
            result["LineCode"] = lineCode;
            result["SectionCode"] = sectionCode;
            result["StationCode"] = stationCode;
            result["OPCategory"] = opCategory;
            result["OPRequestInfo"] = JObject.Parse(opRequestInfo);
            result["OPResponseInfo"] = JObject.Parse(opResponseInfo);
            return result.ToString(Formatting.None);
        }

        private static string GenerateOkOpResponseInfo(string opCategory, string opResponseInfo)
        {
            return "\"OPResponseInfo\":{\"Result\":\"OK\"}";
        }
        private static string GetFailOpResponseInfo(string opCategory, string opResponseInfo) 
        {
            JObject opResponseInfoObj;

            if (string.IsNullOrEmpty(opResponseInfo))
            {
                opResponseInfoObj = new JObject();                
            }
            else
            {
                opResponseInfoObj = JObject.Parse(opResponseInfo);
            }
            switch (opCategory.ToUpper())
            {
                case "GET_SN_BY_SN_FIXTURE":
                    opResponseInfoObj.Add("SN", "");
                    break;
                case "GET_INPUT_DATA":
                    opResponseInfoObj.Add("Data", "{}");
                    break;
                case "LOGIN":
                    opResponseInfoObj.Add("Hwd", "");
                    break;
                case "GET_SN_BY_SMTSN":
                    opResponseInfoObj.Add("SN", "");
                    break;
                default:
                    opResponseInfoObj.Add("Result", "NG");
                    break;
            }
            return opResponseInfoObj.ToString(Formatting.None);
        }
    }
}
