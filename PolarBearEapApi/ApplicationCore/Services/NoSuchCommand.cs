using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** 當找不到對應的IMesCommand時，使用NoSuchCommand。Special Case Pattern */
    public class NoSuchCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "NO_SUCH_COMMAND";
        async Task<MesCommandResponse> IMesCommand.Execute(MesCommandRequest input)
        {
            return await Task.Run( () => GetResponse(""));
        }

        private MesCommandResponse GetResponse(string mesReturnString)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = ErrorCodeEnum.NoSuchCommand.ToString();

            //return "{\"Result\":\"NG\",\"ResultCoded\":\"NoSuchCommand\"}";
            return response;
        }
    }
}
