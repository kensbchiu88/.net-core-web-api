using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services

{
    public class NoSuchCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "NO_SUCH_COMMAND";
        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            return GetResponse("");
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
