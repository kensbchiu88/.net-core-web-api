using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** MES Command Interface */
    public interface IMesCommand
    {
        string CommandName { get; }
        Task<MesCommandResponse> Execute(MesCommandRequest input);
    }
}
