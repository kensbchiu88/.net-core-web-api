using Microsoft.AspNetCore.Mvc;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.PublicApi.Controllers
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
        public ApiResponse Api([FromBody] ApiRequest data)
        {
            var commandName = JsonUtil.GetParameter(data.SerializeData, "OPCategory");
            MesCommandRequest serviceInput = new MesCommandRequest
            {
                Hwd = data.Hwd,
                SerializeData = data.SerializeData
            };

            //excute
            var command = _mesCommandFactory.Get(commandName ?? string.Empty);
            MesCommandResponse serviceReturn = command.Execute(serviceInput);

            //construct response
            ApiResponse response = new ApiResponse
            {
                Indicator = data.Indicator,
                Hwd = data.Hwd
            };
            response.SerializeData = ResponseGenerator.WithOpResponseInfoJson(data.SerializeData, serviceReturn.OpResponseInfo);
            if (serviceReturn.ErrorMessage != null)
            {
                response.Display = serviceReturn.ErrorMessage;
            }

            return response;
        }
    }
}
