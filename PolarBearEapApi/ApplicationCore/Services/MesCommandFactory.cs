using PolarBearEapApi.ApplicationCore.Interfaces;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** IMesCommandFactory 實作 */
    public class MesCommandFactory<T> : IMesCommandFactory<T> where T : IMesCommand
    {
        private readonly IEnumerable<T> _commands;
        public MesCommandFactory(IEnumerable<T> commands)
        {
            _commands = commands;
        }

        /** 依據傳入的字串回傳對應的IMesCommand */
        public T Get(string name)
        {
            int count = _commands.Where(c => c.CommandName.Equals(name.ToUpper())).Count();
            if (count > 0)
            {
                return _commands.Single(c => c.CommandName.Equals(name.ToUpper()));
            }
            return _commands.Single(c => c.CommandName.Equals("NO_SUCH_COMMAND"));
        }
    }
}
