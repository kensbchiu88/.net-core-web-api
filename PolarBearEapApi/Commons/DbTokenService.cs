using FIT.MES.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.Commons.Exceptions;
using PolarBearEapApi.Repository;

namespace PolarBearEapApi.Commons
{
    public class DbTokenService : ITokenService
    {
        private readonly EapTokenDbContext _context;
        private readonly ILogger<DbTokenService> _logger;
        private readonly IConfigCacheService _cacheService;
        private const int _DEFAULT_EXPIRED_HOURS = -4;

        public DbTokenService(EapTokenDbContext context, ILogger<DbTokenService> logger, IConfigCacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //throw InvalidOperationException, TokenExpireException
        public void Validate(string id)
        {
            int expiredHours = GetExpiredHours();

            var expiredTimeString = _cacheService.GetValue("TokenExpireHours");

            Guid tokenId = new Guid(id);

            var tokens = _context.EapTokenEntities.Where(e => e.Id == tokenId);

            if (tokens.Any())
            {
                if (tokens.First().LoginTime.CompareTo(DateTime.Now.AddHours(expiredHours)) < 0)
                {
                    throw new TokenExpireException("TokenExpired:" + id);
                }
            }
            else
            {
                throw new InvalidTokenException("Ivalid Token");
            }
        }

        public string Create(string username)
        {
            //insert db
            Guid id = Guid.NewGuid();

            var entity = new EapTokenEntity();
            entity.Id = id;
            entity.username = username;
            entity.LoginTime = DateTime.Now;
            _context.EapTokenEntities.Add(entity);
            _context.SaveChangesAsync();
            return id.ToString();
        }

        public TokenInfo GetTokenInfo(string id) 
        {
            Guid tokenId = new Guid(id);

            var tokens = _context.EapTokenEntities.Where(e => e.Id == tokenId);

            if (tokens.Any())
            {
                return EapTokenEntity.ConvertToTokenInfo(tokens.First());
            }
            else
            {
                throw new InvalidTokenException("Ivalid Token");
            }
        }

        public void BindMachine(string id, string lineCode, string sectionCode, string stationCode, string serverVersion)
        {
            Guid tokenId = new Guid(id);

            var tokens = _context.EapTokenEntities.Where(e => e.Id == tokenId);

            if (tokens.Any())
            {
                var token = tokens.First();
                token.LineCode = lineCode;
                token.SectionCode = sectionCode;
                token.StationCode = int.Parse(stationCode ?? "0");
                token.ServerVersion = serverVersion;
                token.BindTime = DateTime.Now;
                _context.Update(token);
                _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidTokenException("Ivalid Token");
            }
        }

        private int GetExpiredHours()
        {
            int expiredHours = _DEFAULT_EXPIRED_HOURS;

            var expiredTimeString = _cacheService.GetValue("TokenExpireHours");

            if (!string.IsNullOrEmpty(expiredTimeString))
            {
                try
                {
                    expiredHours = 0 - int.Parse(expiredTimeString);
                }
                catch
                {
                    _logger.LogError("Parse TokenExpireHours fail. value=" + expiredTimeString);
                }
            }
            return expiredHours;
        }
    }
}
