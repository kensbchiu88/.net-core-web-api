using Azure;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;

namespace PolarBearEapApi.Models
{
    public class MesCommandResponse
    {
        public string OpResponseInfo { get; set; }
        public string? ErrorMessage { get; set; }


        /** 
         * 建構子
         * mesReturnString: FIT.MES.Service回傳的Json String
         */
        public MesCommandResponse (string mesReturnString) 
        {
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturnString);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    this.OpResponseInfo = "{\"Result\":\"OK\"}";
                }
                else
                {
                    this.OpResponseInfo = "{\"Result\":\"NG\"}";
                    this.ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                this.OpResponseInfo = "{\"Result\":\"NG\"}";
                this.ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
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
                    this.OpResponseInfo = opResponseInfo;
                }
                else
                {
                    this.OpResponseInfo = "{\"Result\":\"NG\"}";
                    this.ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                this.OpResponseInfo = "{\"Result\":\"NG\"}";
                this.ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }
        }

        public MesCommandResponse()
        {
        }
    }
}
