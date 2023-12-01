using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** UNIT_PROCESS_CHECK_SMT，依據SN檢查路由，確認產品是否允許過站 for SMT */
    public class UnitProcessCheckSmtCommand : IMesCommand
    {
        public string CommandName { get; } = "UNIT_PROCESS_CHECK_SMT";

        private readonly IMesService _equipmentService;

        public UnitProcessCheckSmtCommand(IMesService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {            
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            string unitProcessCheckReturn = string.Empty;
            string getBadmarkReturn = string.Empty;
            try
            {
                unitProcessCheckReturn = await _equipmentService.UNIT_PROCESS_CHECK(inputModel.OPRequestInfo.Sn!, inputModel.SectionCode!, inputModel.StationCode.ToString()!);
                getBadmarkReturn = await _equipmentService.GET_BADMARK(inputModel.OPRequestInfo.Sn!);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }

            return GetResponse(unitProcessCheckReturn, getBadmarkReturn);
        }

        private MesCommandResponse GetResponse(string unitProcessCheckReturn, string getBadmarkReturn)
        {           
            string errorMessage = string.Empty;
            string result = "NG";
            string badmark = string.Empty;

            //parse unit process check
            var unitPorcessCheckResponse = JsonConvert.DeserializeObject<FITMesResponse>(unitProcessCheckReturn);
            if (unitPorcessCheckResponse != null && unitPorcessCheckResponse.Result != null && "OK".Equals(unitPorcessCheckResponse.Result.ToUpper()))
            {
                result = "OK";
            }
            else
            {
                errorMessage = unitPorcessCheckResponse!.Display!;
            }

            //parse bad mark
            var badmarkResponse = JsonConvert.DeserializeObject<FITMesResponse>(getBadmarkReturn);
            if (badmarkResponse != null && badmarkResponse.Result != null && "OK".Equals(badmarkResponse.Result.ToUpper()))
            {
                badmark = badmarkResponse.ResultCode!;
                if (!string.IsNullOrEmpty(badmark))
                {
                    badmark = badmark.Replace(':', ',');
                }
            }
            else
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = badmarkResponse!.Display!;
                }
                else
                {
                    errorMessage = errorMessage  + "; " + badmarkResponse!.Display;
                }                
            }

            //convert to MesCommandResponse
            JObject opResponseInfoObj = new JObject();
            opResponseInfoObj.Add("Result", result);
            opResponseInfoObj.Add("BadMark", badmark);

            MesCommandResponse response = new MesCommandResponse
            {
                OpResponseInfo = opResponseInfoObj.ToString(Formatting.None),
                ErrorMessage = errorMessage
            };

            return response;
        }

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.SectionCode))
                requiredFields.Add("SectionCode");
            if (inputModel.StationCode == null)
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Sn))
                requiredFields.Add("OPRequestInfo.SN");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class OpRequestInfoModel
        {
            public string? Sn { get; set; }
        }

        private class SerializeDataModel
        {
            public string? LineCode { get; set; }
            public string? SectionCode { get; set; }
            public int? StationCode { get; set; }
            [Required]
            public OpRequestInfoModel? OPRequestInfo { get; set; }
        }
    }    
}
