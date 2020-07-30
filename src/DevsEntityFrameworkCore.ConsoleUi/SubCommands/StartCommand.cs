using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName = "start", Name = "start", Description = "Used to prepare the solution")]
    public class StartCommand : OptionsCommandBase
    {
        private readonly IStartService _startService;
        private readonly IOptionsCommand _optionsCommand;

        public StartCommand(
            ILoggerFactory logger,
            IConsole console,
            IStartService startService,
            IOptionsCommand optionsCommand)
            : base(logger, console)
        {
            _startService = startService;
            _optionsCommand = optionsCommand;
        }

        protected override async Task<int> OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _optionsCommand.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _optionsCommand.ReplaceFile = OptionReplaceFile.Value;

            await _startService.Handler();
            Logger.LogTrace("Start Finished");

            return await base.OnExecute(application);
        }
    }
}
