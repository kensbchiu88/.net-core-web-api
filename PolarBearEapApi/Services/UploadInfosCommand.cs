using PolarBearEapApi.Commons;
using PolarBearEapApi.Repository;
using System.Net.Http.Headers;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class UploadInfosCommand : IMesCommand
    {
        private readonly UploadInfoDbContext _uploadInfoDbContext;

        string IMesCommand.CommandName { get; } = "UPLOAD_INFOS";

        public UploadInfosCommand(UploadInfoDbContext uploadInfoDbContext)
        {
            _uploadInfoDbContext = uploadInfoDbContext;
        }
        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            try {
                string lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
                string sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
                string stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
                string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
                string opRequestInfo = JsonUtil.RemoveSn(JsonUtil.GetParameter(serializedData, "OPRequestInfo")).Replace(System.Environment.NewLine, string.Empty); ;

                //using (var db = new UploadInfoDbContext())
                {
                    var entity = new UploadInfoEntity();
                    entity.LineCode = lineCode;
                    entity.SectionCode = sectionCode;
                    entity.StationCode = int.Parse(stationCode);
                    entity.Sn = sn;
                    entity.OpRequestInfo = opRequestInfo;
                    entity.UploadTime = DateTime.Now;
                    _uploadInfoDbContext.UploadInfoEnties.Add(entity);
                    _uploadInfoDbContext.SaveChanges();
                }
                return Success();
            } catch {
                return Fail();

            }
        }

        private MesCommandResponse Success() {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"OK\"}";
            return response;
        }

        private MesCommandResponse Fail()
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = ErrorCodeEnum.UploadFail.ToString();
            return response;
        }
    }
}
