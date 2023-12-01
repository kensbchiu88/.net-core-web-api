using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApiTests
{
    public class NoSuchCommandTest
    {
        private readonly IMesCommand _command;

        public NoSuchCommandTest() 
        { 
            _command = new NoSuchCommand();
        }

        [Fact]
        public async Task Test()
        {
            MesCommandResponse response = await _command.Execute(new MesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.NoSuchCommand.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }
    }
}
