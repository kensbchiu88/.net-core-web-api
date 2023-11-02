using FIT.MES.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.Infra;
using PolarBearEapApi.ApplicationCore.Constants;

namespace PolarBearEapApi.Infra.Services
{
    public class DbTokenRepository : ITokenRepository
    {
        private readonly EapTokenDbContext _context;
        private readonly ILogger<DbTokenRepository> _logger;
        private readonly IConfigCacheService _cacheService;
        private const int _DEFAULT_EXPIRED_HOURS = -4;

        public DbTokenRepository(EapTokenDbContext context, ILogger<DbTokenRepository> logger, IConfigCacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //throw InvalidOperationException, TokenExpireException
        public async Task Validate(string id)
        {
            ValidateTokenFormat(id);

            int expiredHours = GetExpiredHours();

            Guid tokenId = new Guid(id);

            var tokens = await _context.EapTokenEntities.Where(e => e.Id == tokenId).ToListAsync() ;

            if (tokens.Any())
            {
                var token = tokens.First();
                if (expiredHours > 0 && token.LoginTime.CompareTo(DateTime.Now.AddHours(0 - expiredHours)) < 0)
                {
                    throw new EapException(ErrorCodeEnum.TokenExpired);
                }
                else if (token.CardTime != null)
                {
                    throw new EapException(ErrorCodeEnum.TokenExpired);
                }
                else if(token.IsInvalid == true) 
                {
                    throw new EapException(ErrorCodeEnum.InvalidToken);
                }
            }
            else
            {
                throw new EapException(ErrorCodeEnum.InvalidToken);
            }
        }

        public async Task<string> Create(string username)
        {
            //insert db
            Guid id = Guid.NewGuid();

            var entity = new EapTokenEntity();
            entity.Id = id;
            entity.username = username;
            entity.LoginTime = DateTime.Now;
            _context.EapTokenEntities.Add(entity);
            await _context.SaveChangesAsync();
            return id.ToString();
        }

        public async Task<TokenInfo> GetTokenInfo(string id)
        {
            ValidateTokenFormat(id);

            Guid tokenId = new Guid(id);

            var tokens = await _context.EapTokenEntities.Where(e => e.Id == tokenId).ToListAsync();

            if (tokens.Any())
            {
                return EapTokenEntity.ConvertToTokenInfo(tokens.First());
            }
            else
            {
                throw new EapException(ErrorCodeEnum.InvalidToken);
            }
        }

        public async Task BindMachine(string id, string lineCode, string sectionCode, string stationCode, string serverVersion)
        {
            ValidateTokenFormat(id);

            Guid tokenId = new Guid(id);

            var tokens = await _context.EapTokenEntities.Where(e => e.Id == tokenId).ToListAsync();

            if (tokens.Any())
            {
                var token = tokens.First();
                token.LineCode = lineCode;
                token.SectionCode = sectionCode;
                token.StationCode = int.Parse(stationCode ?? "0");
                token.ServerVersion = serverVersion;
                token.BindTime = DateTime.Now;
                _context.Update(token);
                _context.SaveChanges();
            }
            else
            {
                throw new EapException(ErrorCodeEnum.InvalidToken);
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
                    expiredHours = int.Parse(expiredTimeString);
                }
                catch
                {
                    _logger.LogError("Parse TokenExpireHours fail. value=" + expiredTimeString);
                }
            }
            return expiredHours;
        }

        private static void ValidateTokenFormat(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new EapException(ErrorCodeEnum.InvalidTokenFormat, "Token is empty");
            if (!Guid.TryParse(id, out var newGuid))
                throw new EapException(ErrorCodeEnum.InvalidTokenFormat, "Token format should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)");
        }
    }
}
