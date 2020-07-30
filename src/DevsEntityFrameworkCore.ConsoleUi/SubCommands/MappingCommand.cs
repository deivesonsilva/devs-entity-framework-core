using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName="mapping", Name="map", Description="Used to create the objects database configuration")]
    public class MappingCommand : OptionsCommandBase
    {
        private readonly IMappingService _mappingService;
        private readonly IOptionsCommand _optionsCommand;

        public MappingCommand(
            ILoggerFactory logger,
            IConsole console,
            IMappingService mappingService,
            IOptionsCommand optionsCommand) : base(logger, console)
        {
            _mappingService = mappingService;
            _optionsCommand = optionsCommand;
        }

        protected override async Task<int> OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _optionsCommand.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _optionsCommand.ReplaceFile = OptionReplaceFile.Value;

            await _mappingService.Handler();
            Logger.LogTrace("Mapping Finished");

            return await base.OnExecute(application);
        } 
    }
}
