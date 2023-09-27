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

            var commandName = JsonUtil.GetParameter(data.SerializeData, "OPCategory");

            //excute
            var command = _mesCommandFactory.Get(commandName);
            MesCommandResponse serviceReturn = command.Execute(data.SerializeData);

            ApiResponse response = new ApiResponse();
            response.Indicator = data.Indicator;
            response.SerializeData = ResponseGenerator.WithOpResponseInfoJson(data.SerializeData, serviceReturn.OpResponseInfo);
            if (serviceReturn.ErrorMessage != null) { 
                response.Display = serviceReturn.ErrorMessage;
            }

            _logger.LogInformation("Api Response:" + JsonConvert.SerializeObject(response));
            return response;
        }
    }
}
