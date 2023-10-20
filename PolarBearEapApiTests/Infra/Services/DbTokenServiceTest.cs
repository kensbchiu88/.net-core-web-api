using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.Infra;
using PolarBearEapApi.Infra.Services;

namespace PolarBearEapApiUnitTests.Infra.Services
{
    public class DbTokenServiceTest
    {
        private readonly EapTokenDbContext _context;
        private readonly Mock<ILogger<DbTokenService>> _logger;
        private readonly Mock<IConfigCacheService> _cacheService;

        private const string TOKEN_EXPIRED_HOURS = "4";
        private const string USERNAME = "username";
        private const string LINE_CODE = "LINE_CODE";
        private const string SECTION_CODE = "SECTION_CODE";
        private const string STATION_CODE = "11111";
        private const string SERVER_VERSION = "1.01";
        private const string FAKE_TOKEN_ID = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";


        public DbTokenServiceTest() 
        {            
            var options = new DbContextOptionsBuilder<EapTokenDbContext>().UseInMemoryDatabase(databaseName: "Token Unit Test").Options;
            _context = new EapTokenDbContext(options);
            _logger = new Mock<ILogger<DbTokenService>>();
            _cacheService = new Mock<IConfigCacheService>();
        }

        /** 
         * 測試呼叫Validate()，但id = ""
         * Given: id = ""
         * Then: throw InvalidTokenFormatException
         */
        [Fact]
        public void TestValidateEmptyToken()
        {
            var service = new DbTokenService(_context, null, null);
            var caughtException = Assert.Throws<InvalidTokenFormatException>(() => service.Validate(""));
            //Assert.Equal("user field is required", caughtException.Message);
        }

        /** 
         * 測試呼叫Validate()，但id = null
         * Given: id = null
         * Then: throw InvalidTokenFormatException
         */
        [Fact]
        public void TestValidateNullToken()
        {
            var service = new DbTokenService(_context, null, null);
            string? id = null;
            Assert.Throws<InvalidTokenFormatException>(() => service.Validate(id!));
        }

        /** 
         * 測試呼叫Validate()，但id格式錯誤
         * Given: id = "Hello"
         * Then: throw InvalidTokenFormatException
         */
        [Fact]
        public void TestValidateWrongFormatToken()
        {
            var service = new DbTokenService(_context, null, null);
            Assert.Throws<InvalidTokenFormatException>(() => service.Validate("Hello"));
        }

        /** 
         * 測試呼叫Validate()，但id不存在
         * Given: id = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f"
         * Then: throw InvalidTokenFormatException
         */
        [Fact]
        public void TestValidateInvalidToken()
        {
            _cacheService.Setup(service => service.GetValue(It.IsAny<string>())).Returns(TOKEN_EXPIRED_HOURS);
            var service = new DbTokenService(_context, _logger.Object, _cacheService.Object);
            Assert.Throws<InvalidTokenException>(() => service.Validate(FAKE_TOKEN_ID));
        }

        /** 
         * 測試Validate過期Token
         * Given: 在DB中新增一筆過期的token資料
         * Then: throw TokenExpireException
         */
        [Fact]
        public async void TestValidateExpiredToken()
        {
            //在DB中新增測試資料
            var entity = FakeExpiredEapTokenEntity();
            _context.EapTokenEntities.Add(entity);
            await _context.SaveChangesAsync();

            _cacheService.Setup(service => service.GetValue(It.IsAny<string>())).Returns(TOKEN_EXPIRED_HOURS);
            var service = new DbTokenService(_context, _logger.Object, _cacheService.Object);
            Assert.Throws<TokenExpireException>(() => service.Validate(entity.Id.ToString()));
        }

        /** 
         * 測試Validate正確的Token
         * Given: 在DB中新增一筆未過期的token資料
         * Then: 沒有丟出Exception
         */
        [Fact]
        public async void TestValidateValidToken()
        {
            //在DB中新增測試資料
            var entity = FakeEapTokenEntity();
            _context.EapTokenEntities.Add(entity);
            await _context.SaveChangesAsync();

            _cacheService.Setup(service => service.GetValue(It.IsAny<string>())).Returns(TOKEN_EXPIRED_HOURS);
            var service = new DbTokenService(_context, _logger.Object, _cacheService.Object);
            var exception = Record.Exception(() => service.Validate(entity.Id.ToString()));
            Assert.Null(exception);
        }

        /** 
         * 測試Create token
         * Given: username
         * Then: DB中存在該Guid的資料，且username與input相同
         */
        [Fact]
        public async void TestCreateToken()
        {
            var service = new DbTokenService(_context, null, null);
            var id = service.Create(USERNAME);

            //驗證DB中的資料
            var entities = _context.EapTokenEntities.Where(e => e.Id.ToString() == id).ToList();
            Assert.Single(entities);
            Assert.Equal(id, entities[0].Id.ToString());
            Assert.Equal(USERNAME, entities[0].username);

            //移除DB中測試資料，避免影響其他測試
            if (entities != null)
            {
                _context.EapTokenEntities.Remove(entities[0]);
                await _context.SaveChangesAsync();
            }
        }

        /** 
         * 測試嘗試抓取不存在的token
         * Given: 不存在的token id
         * Then: 丟出InvalidTokenException
         */
        [Fact]
        public void TestGetTokenInfoWithInvalidTokenId()
        {
            //_cacheService.Setup(service => service.GetValue(It.IsAny<string>())).Returns(TOKEN_EXPIRED_HOURS);
            var service = new DbTokenService(_context, null, null);
            Assert.Throws<InvalidTokenException>(() => service.GetTokenInfo(FAKE_TOKEN_ID));
        }

        /** 
         * 測試抓取存在的token
         * Given: 存在的token id
         * Then: 回傳TokenInfo，且Id與input相同
         */
        [Fact]
        public async void TestGetTokenInfo()
        {
            //在DB中新增測試資料
            var entity = FakeEapTokenEntity();
            _context.EapTokenEntities.Add(entity);
            await _context.SaveChangesAsync();

            var service = new DbTokenService(_context, null, null);
            var token = service.GetTokenInfo(entity.Id.ToString());

            Assert.NotNull(token);
            Assert.Equal(entity.Id.ToString(), token.Id);

            //移除DB中測試資料，避免影響其他測試
            _context.EapTokenEntities.Remove(entity);
            await _context.SaveChangesAsync();            
        }

        /** 
         * 成功BindMachine
         * Given: 存在的token
         * Then: 驗證資料庫中的資料，id, line_code, section_code, machine_code, server_version 與input相同, 且bind_time不為空。且不丟出Exception
         */
        [Fact]
        public async void TestBindMachine()
        {
            //在DB中新增測試資料
            var entity = FakeEapTokenEntity();
            _context.EapTokenEntities.Add(entity);
            await _context.SaveChangesAsync();

            var service = new DbTokenService(_context, null, null);
            var exception = Record.Exception(() => service.BindMachine(entity.Id.ToString(), LINE_CODE, SECTION_CODE, STATION_CODE, SERVER_VERSION));

            Assert.Null(exception);

            //驗證DB中的資料
            var entities = _context.EapTokenEntities.Where(e => e.Id == entity.Id).ToList();
            Assert.Single(entities);
            Assert.Equal(entity.Id, entities[0].Id);
            Assert.Equal(LINE_CODE, entities[0].LineCode);
            Assert.Equal(SECTION_CODE, entities[0].SectionCode);
            Assert.Equal(STATION_CODE, entities[0].StationCode.ToString());
            Assert.Equal(SERVER_VERSION, entities[0].ServerVersion);
            Assert.NotNull(entities[0].BindTime);

            //移除DB中測試資料，避免影響其他測試
            _context.EapTokenEntities.Remove(entities[0]);
            await _context.SaveChangesAsync();
        }

        /** 
         * 測試不存在的token執行BindMachine
         * Given: 不存在的token
         * Then: 丟出InvalidTokenException
         */
        [Fact]
        public void TestBindMachineWithInvalidToken()
        {
            var service = new DbTokenService(_context, null, null);
            Assert.Throws<InvalidTokenException>(() => service.BindMachine(FAKE_TOKEN_ID, LINE_CODE, SECTION_CODE, STATION_CODE, SERVER_VERSION));
        }

        private static EapTokenEntity FakeExpiredEapTokenEntity()
        {
            var expiredHours = int.Parse(TOKEN_EXPIRED_HOURS);
            EapTokenEntity entity = new EapTokenEntity();
            entity.Id = Guid.NewGuid();
            entity.username = USERNAME;
            entity.LoginTime = DateTime.Now.AddHours(0 - expiredHours - 1);
            
            return entity;
        }

        private static EapTokenEntity FakeEapTokenEntity()
        {
            EapTokenEntity entity = new EapTokenEntity();
            entity.Id = Guid.NewGuid();
            entity.username = USERNAME;
            entity.LoginTime = DateTime.Now;

            return entity;
        }
    }
}
