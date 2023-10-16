using Azure;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;

namespace PolarBearEapApi.PublicApi.Models
{
    public class MesCommandResponse
    {
        public string OpResponseInfo { get; set; }
        public string? ErrorMessage { get; set; }


        /** 
         * 建構子
         * mesReturnString: FIT.MES.Service回傳的Json String
         */
        public MesCommandResponse(string mesReturnString)
        {
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturnString);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    OpResponseInfo = "{\"Result\":\"OK\"}";
                }
                else
                {
                    OpResponseInfo = "{\"Result\":\"NG\"}";
                    ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                OpResponseInfo = "{\"Result\":\"NG\"}";
                ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }
        }

        /** 
         * 建構子
         * mesReturnString: FIT.MES.Service回傳的Json String
         * opResponseInfo: 客製化的OpResponseInfo
         */
        public MesCommandResponse(string mesReturnString, string opResponseInfo)
        {
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturnString);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    OpResponseInfo = opResponseInfo;
                }
                else
                {
                    OpResponseInfo = "{\"Result\":\"NG\"}";
                    ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                OpResponseInfo = "{\"Result\":\"NG\"}";
                ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }
        }

        public MesCommandResponse()
        {
        }

        public static MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = errorCodeEnum.ToString();
            return response;
        }

        public static MesCommandResponse Ok()
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"OK\"}";
            return response;
        }
    }
}
