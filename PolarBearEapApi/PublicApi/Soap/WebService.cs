using PolarBearEapApi.ApplicationCore.Interfaces;
using Serilog;

namespace PolarBearEapApi.PublicApi.Soap
{
    public class WebService : IWebService
    {
        private readonly IAuthenticationService _authenticationService;

        public WebService(IAuthenticationService authenticationService)
        { 
            _authenticationService = authenticationService;
        }

        public async Task<bool> UserLogin(string user, string passwrod)
        {
            return await _authenticationService.Login(user, passwrod);
        }
    }
}
