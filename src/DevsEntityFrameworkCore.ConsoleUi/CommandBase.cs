using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.ConsoleUi
{
    [HelpOption("--help")]
    public abstract class CommandBase
    {
        protected ILogger Logger { get; }
        protected IConsole Console { get; }

        protected CommandBase(
            ILoggerFactory logger,
            IConsole console)
        {
            Logger = logger.CreateLogger(GetType());
            Console = console;
        }

        protected virtual Task<int> OnExecute(CommandLineApplication application)
        {
            return Task.FromResult(0);
        }
    }
}