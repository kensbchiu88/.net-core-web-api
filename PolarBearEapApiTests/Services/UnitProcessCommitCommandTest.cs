using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarBearEapApiTests
{
    public class UnitProcessCommitCommandTest
    {
        private const string MES_RETURN_DISPLAY = "WF002:Routing Error, Goto <TE020>";

        /** 
         * 測試執行成功(提交測試成功的過站紀錄)
         * 測試成功或非測試站的過站紀錄json如下:{"Hwd": "36bd2cd3-3c94-4d53-a494-79eab5d34e9f","Indicator": "ADD_RECORD","SerializeData": "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"TE060\",\"StationCode\":72607,\"OPCategory\":\"UNIT_PROCESS_COMMIT\",\"OPRequestInfo\":{\"SN\":\"H2C336500010VC6RX\"},\"OPResponseInfo\":{}}"}
         * Given: Mes回傳OK
         * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
         */
        [Fact]
        public async Task TestSuccess()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_COMMIT(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new UnitProcessCommitCommand(null!, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"Result\":\"OK\"}", response.OpResponseInfo);
        }

        /** 
         * 測試執行失敗(提交測試成功的過站紀錄)
         * Given: Mes回傳NG
         * Then: 回傳 "{\"Result\":\"NG\"}", ErrorMessage = MES回傳的Display欄位
         */
        [Fact]
        public async Task TestFail()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_COMMIT(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new UnitProcessCommitCommand(null!, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception(提交測試成功的過站紀錄)
         * Given: MES丟出Exception
         * Then: 回傳 Result:"NG", ErrorMessage = CallMesServiceException
         */
        [Fact]
        public async Task TestMesThrowException()
        {
            var mockLogger = new Mock<ILogger<UnitProcessCommitCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_COMMIT(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var command = new UnitProcessCommitCommand(mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.CallMesServiceException.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        /** 
         * 測試執行成功(提交測試失敗的過站紀錄)
         * 測試失敗的過站紀錄json如下:{"Hwd": "36bd2cd3-3c94-4d53-a494-79eab5d34e9f","Indicator": "ADD_RECORD","SerializeData": "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"TE060\",\"StationCode\":72607,\"OPCategory\":\"UNIT_PROCESS_COMMIT\",\"OPRequestInfo\":{\"SN\":\"H2C336500010VC6RX\",\"result\":\"FAIL\",\"list_of_failing_tests\":\"errorCode1;errorCode2;errorCode3\",\"failure_message\":\"errorDesc1;errorDesc2;errorDesc3\"},\"OPResponseInfo\":{}}"}
         * Given: Mes回傳OK
         * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
         */
        [Fact]
        public async Task TestFailureCommitSuccess()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_COMMIT(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "FAIL", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new UnitProcessCommitCommand(null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockFailureCommitMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"Result\":\"OK\"}", response.OpResponseInfo);
        }

        /** 
         * 測試執行失敗(提交測試失敗的過站紀錄)
         * Given: Mes回傳NG
         * Then: 回傳 "{\"Result\":\"NG\"}", ErrorMessage = MES回傳的Display欄位
         */
        [Fact]
        public async Task TesttFailureCommitSFail()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_COMMIT(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "FAIL", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new UnitProcessCommitCommand(null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockFailureCommitMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception(提交測試失敗的過站紀錄)
         * Given: MES丟出Exception
         * Then: 回傳 Result:"NG", ErrorMessage = CallMesServiceException
         */
        [Fact]
        public async Task TestFailureCommitMesThrowException()
        {
            var mockLogger = new Mock<ILogger<UnitProcessCommitCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_COMMIT(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "FAIL", It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var command = new UnitProcessCommitCommand(mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockFailureCommitMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.CallMesServiceException.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        //測試成功或非測試站的過站紀錄json如下:{"Hwd": "36bd2cd3-3c94-4d53-a494-79eab5d34e9f","Indicator": "ADD_RECORD","SerializeData": "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"TE060\",\"StationCode\":72607,\"OPCategory\":\"UNIT_PROCESS_COMMIT\",\"OPRequestInfo\":{\"SN\":\"H2C336500010VC6RX\"},\"OPResponseInfo\":{}}"}
        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"Hwd\": \"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"ADD_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"TE060\\\",\\\"StationCode\\\":72607,\\\"OPCategory\\\":\\\"UNIT_PROCESS_COMMIT\\\",\\\"OPRequestInfo\\\":{\\\"SN\\\":\\\"H2C336500010VC6RX\\\"},\\\"OPResponseInfo\\\":{}}\"}";
            return request;
        }

        //測試失敗的過站紀錄json如下:{"Hwd": "36bd2cd3-3c94-4d53-a494-79eab5d34e9f","Indicator": "ADD_RECORD","SerializeData": "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"TE060\",\"StationCode\":72607,\"OPCategory\":\"UNIT_PROCESS_COMMIT\",\"OPRequestInfo\":{\"SN\":\"H2C336500010VC6RX\",\"result\":\"FAIL\",\"list_of_failing_tests\":\"errorCode1;errorCode2;errorCode3\",\"failure_message\":\"errorDesc1;errorDesc2;errorDesc3\"},\"OPResponseInfo\":{}}"}
        private static MesCommandRequest MockFailureCommitMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"TE060\",\"StationCode\":72607,\"OPCategory\":\"UNIT_PROCESS_COMMIT\",\"OPRequestInfo\":{\"SN\":\"H2C336500010VC6RX\",\"result\":\"FAIL\",\"list_of_failing_tests\":\"errorCode1;errorCode2;errorCode3\",\"failure_message\":\"errorDesc1;errorDesc2;errorDesc3\"},\"OPResponseInfo\":{}}";
            return request;
        }
    }
}
