using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;
using PolarBearEapApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
