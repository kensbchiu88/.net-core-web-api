namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface ILearnFileAlterWarningService
    {
        Task Send(string filePath, string alterTime, string equipment);
    }
}
