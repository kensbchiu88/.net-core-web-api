using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;
using PolarBearEapApi.ApplicationCore.Services;

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
        public void Test()
        {
            MesCommandResponse response = _command.Execute(new MesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.NoSuchCommand.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }
    }
}
