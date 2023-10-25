using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMesService _equipmentService;

        public AuthenticationService(IMesService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public bool Login(string username, string password)
        {
            bool result = false;

            string mesReturn;
            try
            {
                mesReturn = _equipmentService.CHECK_OP_PASSWORD(username, password);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            //驗證MES結果
            if (FITMesResponse.IsResultOk(mesReturn))
            {
                result = true; 
            }

            return result;
        }
    }
}
