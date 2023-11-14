using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Extensions;
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
    public class GetInputDataCommandTest
    {
        private const string LINE_CODE = "LINE_CODE";
        private const string SECTION_CODE = "SECTION_CODE";
        private const int STATION_CODE = 1;
        private const string SN = "SN";
        private const string OP_REQUEST_INFO = "{\"A\": \"a\",  \"B\": \"b\",  \"C\": \"c\"}";



        /** 
         * 測試執行成功
         * Given: IUploadInfoService回傳新增的UploadInfoEntity
         * Then: 回傳 "{\"Data\":\"{\"A\": \"a\",  \"B\": \"b\",  \"C\": \"c\"}\"}", ErrorMessage = null
         */
        [Fact]
        public async Task TestSuccess()
        {
            var mockUploadInfoService = new Mock<IUploadInfoRepository>();

            mockUploadInfoService.Setup(service => service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(MockUploadInfoEntity());

            var command = new GetInputDataCommand(null, mockUploadInfoService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            var data = JsonUtil.GetCaseSensitiveParameter(response.OpResponseInfo, "Data");
            Assert.Equal("a", JsonUtil.GetCaseSensitiveParameter(data!, "A"));
            Assert.Equal("b", JsonUtil.GetCaseSensitiveParameter(data!, "B"));
            Assert.Equal("c", JsonUtil.GetCaseSensitiveParameter(data!, "C"));

        }

        /** 
         * 測試no data found
         * Given: IUploadInfoService 回傳NULL
         * Then: 回傳 "{\"Data\":\"{}\"}", ErrorMessage = UploadFail
         */
        [Fact]
        public async Task TestNoDataFound()
        {
            var mockLogger = new Mock<ILogger<GetInputDataCommand>>();
            var mockUploadInfoService = new Mock<IUploadInfoRepository>();

            mockUploadInfoService.Setup(service => service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((string lineCode, string sectionCode, int stationCode, string sn) => null);

            var command = new GetInputDataCommand(mockLogger.Object, mockUploadInfoService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.NoDataFound.ToString(), response.ErrorMessage);
            Assert.Equal("{}", JsonUtil.GetParameter(response.OpResponseInfo, "Data"));
        }

        /** 
         * 測試DB丟出Exception
         * Given:IUploadInfoService丟出Exception
         * Then: 回傳 "{\"Data\":\"{}\"}", ErrorMessage = UploadFail
         */
        [Fact]
        public async Task TestQueryDbException()
        {
            var mockLogger = new Mock<ILogger<GetInputDataCommand>>();
            var mockUploadInfoService = new Mock<IUploadInfoRepository>();

            mockUploadInfoService.Setup(service => service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Throws<Exception>();

            var command = new GetInputDataCommand(mockLogger.Object, mockUploadInfoService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.QueryDbError.ToString(), response.ErrorMessage);
            Assert.Equal("{}", JsonUtil.GetParameter(response.OpResponseInfo, "Data"));
        }
       
        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"" + LINE_CODE + "\",\"SectionCode\":\"" + SECTION_CODE + "\",\"StationCode\":" + STATION_CODE + ",\"OPCategory\":\"UPLOAD_INFOS\",\"OPRequestInfo\":" + OP_REQUEST_INFO + ",\"OPResponseInfo\":{}}";
            return request;
        }

        private static UploadInfoEntity MockUploadInfoEntity()
        {
            UploadInfoEntity entity = new UploadInfoEntity();
            entity.Id = 1;
            entity.LineCode = LINE_CODE;
            entity.SectionCode = SECTION_CODE;
            entity.StationCode = STATION_CODE;
            entity.Sn = SN;
            entity.OpRequestInfo = OP_REQUEST_INFO;

            return entity;

        }
    }
}
