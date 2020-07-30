using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName="repository", Name="repo", Description="Used to create all repository interface")]
    public class RepositoryCommand : OptionsCommandBase
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IOptionsCommand _optionsCommand;

        public RepositoryCommand(
            ILoggerFactory logger,
            IConsole console,
            IRepositoryService repositoryService,
            IOptionsCommand optionsCommand) : base(logger, console)
        {
            _repositoryService = repositoryService;
            _optionsCommand = optionsCommand;
        }

        protected override async Task<int> OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _optionsCommand.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _optionsCommand.ReplaceFile = OptionReplaceFile.Value;
            
            await _repositoryService.Handler();
            Logger.LogTrace("Repository Finished");

            return await base.OnExecute(application);
        }
    }
}
