namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IMesCommandFactory<T> where T : IMesCommand
    {
        T Get(string name);
    }
}
