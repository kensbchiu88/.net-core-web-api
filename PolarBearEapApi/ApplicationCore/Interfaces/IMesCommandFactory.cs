namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /* Mes Command Factory */
    public interface IMesCommandFactory<T> where T : IMesCommand
    {
        T Get(string name);
    }
}
