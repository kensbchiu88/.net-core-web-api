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
        private readonly ICacheService _cacheService;
        private const int _DEFAULT_EXPIRED_HOURS = -4;

        public DbTokenService(EapTokenDbContext context, ILogger<DbTokenService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //throw InvalidOperationException, TokenExpireException
        public void Validate(string tokenString)
        {
            int expiredHours = GetExpiredHours();

            var expiredTimeString = _cacheService.TryGetValue("TokenExpireHours");

            Guid tokenId = new Guid(tokenString);

            var tokens = _context.EapTokenEntities.Where(e => e.Id == tokenId);

            if (tokens.Any())
            {
                if (tokens.First().LoginTime.CompareTo(DateTime.Now.AddHours(expiredHours)) < 0)
                {
                    throw new TokenExpireException("TokenExpired:" + tokenString);
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
            _context.SaveChanges();
            return id.ToString();
        }

        private int GetExpiredHours()
        {
            int expiredHours = _DEFAULT_EXPIRED_HOURS;

            var expiredTimeString = _cacheService.TryGetValue("TokenExpireHours");

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
