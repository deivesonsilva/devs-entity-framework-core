using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DevsEntityFrameworkCore.ConsoleUi
{
    [HelpOption("--help")]
    public abstract class CommandBase
    {
        protected virtual Task OnExecute(CommandLineApplication application)
        {
            return Task.CompletedTask;
        }
    }
}