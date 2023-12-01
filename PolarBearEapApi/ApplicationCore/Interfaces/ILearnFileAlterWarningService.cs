namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** 學習庫變更的 Interface */
    public interface ILearnFileAlterWarningService
    {
        Task Send(string filePath, string alterTime, string equipment);
    }
}
