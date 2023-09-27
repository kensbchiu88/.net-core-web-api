using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PolarBearEapApi.Commons
{
    public class JsonUtil
    {
        public static string GetParameter(string serializeData, string name) {
            if(string.IsNullOrEmpty(serializeData) || string.IsNullOrEmpty(name)){
                return string.Empty;

            }
            /*
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy() // Use CamelCaseNamingStrategy for case-insensitive parsing
                }
            };
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(serializeData, setting);
            */
            string queryString = "$." + name.ToUpper();
            JObject jObj = JObject.Parse(serializeData.ToUpper());

            JToken? jToken = jObj.SelectToken(queryString);
            if(jToken == null)
            {
                return string.Empty;
            }

            return jToken.ToString();

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
