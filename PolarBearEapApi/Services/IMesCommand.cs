using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public interface IMesCommand
    {
        string CommandName { get; }
        MesCommandResponse Execute(string serializedData);
    }
}
