using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;
using PolarBearEapApi.Services;
using System.Diagnostics;

namespace PolarBearEapApi.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class EapApiController : ControllerBase
    {
        private readonly ILogger<EapApiController> _logger;
        private readonly IMesCommandFactory<IMesCommand> _mesCommandFactory;

        public EapApiController(ILogger<EapApiController> logger, IMesCommandFactory<IMesCommand> mesCommandFactory)
        {
            _logger = logger;
            _mesCommandFactory = mesCommandFactory;
        }

        [HttpPost]
        public ApiResponse Api([FromBody] ApiRequest data) {

            _logger.LogInformation("Api Request :" + JsonConvert.SerializeObject(data));
            //Debug.WriteLine(data.SerializeData);

            /*
            JObject serializeData = JObject.Parse(data.SerializeData);
            Debug.WriteLine("line code 1:" + serializeData["LineCode"]);

            var a = JsonConvert.DeserializeObject<SerializeData>(data.SerializeData);
            Debug.WriteLine("line code 2:" + a.LineCode);

            var b = JsonUtil.GetParameter(data.SerializeData, "LineCode");
            Debug.WriteLine("line code 3:" + b);

            var c = JsonUtil.GetParameter(data.SerializeData, "OPRequestInfo.REF_VALUE");
            Debug.WriteLine("REF_VALUE:" + c);
            */

            var commandName = JsonUtil.GetParameter(data.SerializeData, "OPCategory");

            //excute
            //MesCommandResponse serviceReturn = ExecuteMesCommand(data.SerializeData, command);
            var command = _mesCommandFactory.Get(commandName);
            MesCommandResponse serviceReturn = command.Execute(data.SerializeData);

            ApiResponse response = new ApiResponse();
            //response.Cmd = data.Cmd;
            //response.Hwd = data.Hwd;
            response.Indicator = data.Indicator;
            response.SerializeData = ResponseGenerator.WithOpResponseInfoJson(data.SerializeData, serviceReturn.OpResponseInfo);
            if (serviceReturn.ErrorMessage != null) { 
                response.Display = serviceReturn.ErrorMessage;
            }

            _logger.LogInformation("Api Response:" + JsonConvert.SerializeObject(response));
            return response;
        }
        /*
        private MesCommandResponse ExecuteMesCommand(string serializeData, string command) {
            MesCommandResponse result = new MesCommandResponse();
            switch (command.ToUpper()) {
                case "GET_SN_BY_SN_FIXTURE":
                    result = ExecuteGetSnBySnFixture(serializeData);
                    break;
                case "UPLOAD_INFOS":
                    result = ExecuteUploadInfos(serializeData);
                    break;
                case "GET_INPUT_DATA":
                    result = ExecuteGetInputData(serializeData);  
                    break;  
                case "UNIT_PROCESS_CHECK":
                    result = ExecuteUnitProcessCheck(serializeData);
                    break;
                case "ADD_BOM_DATA":
                    result = ExecuteAddBomData(serializeData); 
                    break;
                case "UNIT_PROCESS_COMMIT":
                    result = ExecuteUnitProcessCommit(serializeData);
                    break;
                default:
                    result = NoSuchCommand();
                    break;
            }

            return result;
        }

        private MesCommandResponse ExecuteGetSnBySnFixture(string serializeData) {
            //IMesCommand command = new GetSnBySnFixtureCommand();
            return command.execute(serializeData);
        }

        private MesCommandResponse ExecuteUploadInfos(string serializeData) { 
            IMesCommand command = new UploadInfosCommand();
            return command.Execute(serializeData);
        }

        private MesCommandResponse ExecuteGetInputData(string serializeData)
        {
            IMesCommand command = new GetInputDataCommand();
            return command.Execute(serializeData);
        }

        private MesCommandResponse ExecuteUnitProcessCheck(string serializeData)
        {
            IMesCommand command = new UnitProcessCheckCommand();
            return command.Execute(serializeData);
        }

        private MesCommandResponse ExecuteAddBomData(string serializeData)
        {
            IMesCommand command = new AddBomDataCommand();
            return command.Execute(serializeData);
        }

        private MesCommandResponse ExecuteUnitProcessCommit(string serializeData)
        {
            IMesCommand command = new UnitProcessCommitCommand();
            return command.Execute(serializeData);
        }

        private MesCommandResponse NoSuchCommand()
        {
            IMesCommand command = new NoSuchCommand();
            return command.Execute("");
        }
        */

    }
}
