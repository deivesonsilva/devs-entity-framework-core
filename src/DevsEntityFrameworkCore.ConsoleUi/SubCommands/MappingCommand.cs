using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName="mapping", Name="map", Description="Used to create the objects database configuration")]
    public class MappingCommand : OptionsCommandBase
    {
        private readonly IMappingService _mappingService;
        private readonly IOptionsCommand _options;
        private readonly ICsprojService _csprojService;

        public MappingCommand(
            IOptionsCommand options,
            IMappingService mappingService,
            ICsprojService csprojService)
        {
            _options = options;
            _mappingService = mappingService;
            _csprojService = csprojService;
        }

        protected override async Task OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _options.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _options.ReplaceFile = OptionReplaceFile.Value;

            _csprojService.IsValidProject(_options.DirectoryWorking);

            await _mappingService.CreateMappingFiles();
        } 
    }
}
