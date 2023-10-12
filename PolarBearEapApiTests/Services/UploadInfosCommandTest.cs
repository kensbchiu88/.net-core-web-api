using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;
using PolarBearEapApi.Repository;
using PolarBearEapApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PolarBearEapApiTests
{
    public class UploadInfosCommandTest
    {
        private const string LINE_CODE = "LINE_CODE";
        private const string  SECTION_CODE = "SECTION_CODE";
        private const int STATION_CODE = 1;
        private const string SN = "SN";
        private const string OP_REQUEST_INFO = "{\"SN\":\"" + SN + "\",\"A\": \"a\",  \"B\": \"b\",  \"C\": \"c\"}";



        /** 
         * 測試執行成功
         * Given: IUploadInfoService回傳entity
         * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
         */
        [Fact]
        public void TestSuccess() 
        {
            var mockLogger = new Mock<ILogger<UploadInfosCommand>>();
            var mockUploadInfoService = new Mock<IUploadInfoService>();
            mockUploadInfoService.Setup(service => service.Insert(It.IsAny<UploadInfoEntity>()))
                .Returns(MockUploadInfoEntity);

            var command = new UploadInfosCommand(mockUploadInfoService.Object, null);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"Result\":\"OK\"}", response.OpResponseInfo);
            
        }

        /** 
         * 測試執行失敗
         * Given: IUploadInfoService丟出Exception
         * Then: 回傳 "{\"Result\":\"NG\"}", ErrorMessage = UploadFail
         */
        [Fact]
        public void TestFail()
        {
            var mockLogger = new Mock<ILogger<UploadInfosCommand>>();
            var mockUploadInfoService = new Mock<IUploadInfoService>();

            mockUploadInfoService.Setup(service => service.Insert(It.IsAny<UploadInfoEntity>())).Throws<Exception>();

            var command = new UploadInfosCommand(mockUploadInfoService.Object, mockLogger.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.UploadFail.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }
        
        /*
        [Fact]
        public void TestSuccess()
        {
            var mockLogger = new Mock<ILogger<UploadInfosCommand>>();
            var options = new DbContextOptionsBuilder<UploadInfoDbContext>().UseInMemoryDatabase(databaseName: "Products Test").Options;
            using (var context = new UploadInfoDbContext(options)) 
            {
                var command = new UploadInfosCommand(context, null);
                MesCommandResponse response = command.Execute(MockMesCommandRequest());

                Assert.NotNull(response);
                Assert.Null(response.ErrorMessage);
                Assert.Equal("{\"Result\":\"OK\"}", response.OpResponseInfo);
            }
        }
        */
        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest
            {
                Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57",
                SerializeData = "{\"LineCode\":\"" + LINE_CODE + "\",\"SectionCode\":\"" + SECTION_CODE + "\",\"StationCode\":" + STATION_CODE + ",\"OPCategory\":\"UPLOAD_INFOS\",\"OPRequestInfo\":" + OP_REQUEST_INFO + ",\"OPResponseInfo\":{}}"
            };
            return request;
        }

        private static UploadInfoEntity MockUploadInfoEntity() 
        {
            UploadInfoEntity entity = new UploadInfoEntity
            {
                Id = 1,
                LineCode = LINE_CODE,
                SectionCode = SECTION_CODE,
                StationCode = STATION_CODE,
                Sn = SN,
                OpRequestInfo = OP_REQUEST_INFO
            };

            return entity;
        }
    }
}
