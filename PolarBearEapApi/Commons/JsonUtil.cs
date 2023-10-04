using Newtonsoft.Json.Linq;
using PolarBearEapApi.Commons.Exceptions;

namespace PolarBearEapApi.Commons
{
    public class JsonUtil
    {
        public static string? GetParameter(string serializeData, string name) {
            if(string.IsNullOrEmpty(serializeData) || string.IsNullOrEmpty(name)){
                return string.Empty;
            }

            string queryString = "$." + name.ToUpper();
            try 
            {
                //將Json中的null轉成NULL會出現Newtonsoft.Json.JsonReaderException，故轉大寫後需將NULL轉回null
                JObject jObj = JObject.Parse(serializeData.ToUpper().Replace("NULL", "null"));

                JToken? jToken = jObj.SelectToken(queryString);
                if (jToken == null)
                {
                    return string.Empty;
                }

                return jToken.ToString();
            } 
            catch(Exception ex)
            {
                throw new EapJsonParseException("Json Parse Fail", ex);
            }
        }

        public static string? GetCaseSensitiveParameter(string serializeData, string name)
        {
            if (string.IsNullOrEmpty(serializeData) || string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            string queryString = "$." + name;
            try
            {
                JObject jObj = JObject.Parse(serializeData);

                JToken? jToken = jObj.SelectToken(queryString);
                if (jToken == null)
                {
                    return string.Empty;
                }

                return jToken.ToString();
            }
            catch (Exception ex) 
            {
                throw new EapJsonParseException("Json Parse Fail", ex);
            }
        }

        public static string RemoveSn(string opRequestInfo) 
        {
            JObject o = JObject.Parse(opRequestInfo.ToUpper());

            if(o != null)
            {
                if (o.ContainsKey("SN"))
                    o.Remove("SN");
                return o.ToString();
            }

            return opRequestInfo;
        }

        public static string AddDateTime(string jsonString, string fieldName, string value) {
            JObject o = JObject.Parse(jsonString);
            o[fieldName] = value;
            return o.ToString();
        }
    }
}
