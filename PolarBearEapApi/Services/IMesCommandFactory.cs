namespace PolarBearEapApi.Services
{
    public interface IMesCommandFactory<T> where T : IMesCommand
    {
        T Get(string name);
    }
}
