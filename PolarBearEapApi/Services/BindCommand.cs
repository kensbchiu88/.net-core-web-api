using PolarBearEapApi.Commons;
using PolarBearEapApi.Commons.Exceptions;
using PolarBearEapApi.Models;
using PolarBearEapApi.Repository;

namespace PolarBearEapApi.Services
{
    public class BindCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "BIND";

        private readonly EapTokenDbContext _context;
        private readonly ILogger<BindCommand> _logger;

        public BindCommand(EapTokenDbContext context, ILogger<BindCommand> logger)
        {
            _context = context;
            _logger = logger;
        }

        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? serverVersion = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.ServerVersion");

            //Todo : call mes 檢查職能

            Guid tokenId = new Guid(input.Hwd);

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
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidTokenException("Ivalid Token");
            }

            return MesCommandResponse.Ok();
        }
    }
}
