using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName = "start", Name = "start", Description = "Used to prepare the solution")]
    public class StartCommand : OptionsCommandBase
    {
        private readonly IOptionsCommand _optionsCommand;

        public StartCommand(
            ILoggerFactory logger,
            IConsole console,
            IOptionsCommand optionsCommand)
            : base(logger, console)
        {
            _optionsCommand = optionsCommand;
        }

        protected override async Task<int> OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _optionsCommand.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _optionsCommand.ReplaceFile = OptionReplaceFile.Value;

            Logger.LogTrace("not implemented");

            return await base.OnExecute(application);
        }
    }
}
