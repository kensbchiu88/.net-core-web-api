using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Filters;
using PolarBearEapApi.PublicApi.Models;
using System.Diagnostics;

namespace PolarBearEapApi.PublicApi.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class EapApiController : ControllerBase
    {
        private readonly ILogger<EapApiController> _logger;
        private readonly IMesCommandFactory<IMesCommand> _mesCommandFactory;
        private readonly ILearnFileAlterWarningService _learnFileAlterWarningService;

        public EapApiController(ILogger<EapApiController> logger, IMesCommandFactory<IMesCommand> mesCommandFactory, ILearnFileAlterWarningService learnFileAlterWarningService)
        {
            _logger = logger;
            _mesCommandFactory = mesCommandFactory;
            _learnFileAlterWarningService = learnFileAlterWarningService;
        }

        [HttpPost]
        public async Task<ApiResponse> Api([FromBody] ApiRequest data)
        {
            var commandName = JsonUtil.GetParameter(data.SerializeData, "OPCategory");
            MesCommandRequest serviceInput = new MesCommandRequest
            {
                Hwd = data.Hwd,
                SerializeData = data.SerializeData
            };

            //excute
            var command = _mesCommandFactory.Get(commandName ?? string.Empty);
            MesCommandResponse serviceReturn = await command.Execute(serviceInput);

            //construct response
            ApiResponse response = new ApiResponse
            {
                Indicator = data.Indicator,
                Hwd = data.Hwd
            };
            response.SerializeData = ResponseSerializeDataGenerator.WithOpResponseInfoJson(data.SerializeData, serviceReturn.OpResponseInfo);
            if (serviceReturn.ErrorMessage != null)
            {
                response.Display = serviceReturn.ErrorMessage;
            }
            var responseString = JsonConvert.SerializeObject(response);
            _logger.LogInformation($"Response:{responseString}");
            return response;
        }

        
        [Route("SendLearnFileAlterWarning")]        
        [HttpPost]
        [ServiceFilter(typeof(SimpleResponseRewriteActionFilter))]
        public async Task SendLearnFileAlterWarning([FromBody] SendLearnFileAlterWarningRequest data)
        {            
            await _learnFileAlterWarningService.Send(data.FilePath, data.AlterTime, data.Equipment);
        }
    }
}
