using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName="repository", Name="repo", Description="Used to create all repository interface")]
    public class RepositoryCommand : OptionsCommandBase
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IOptionsCommand _options;
        private readonly ICsprojService _csprojService;

        public RepositoryCommand(
            IRepositoryService repositoryService,
            IOptionsCommand options,
            ICsprojService csprojService)
        {
            _repositoryService = repositoryService;
            _options = options;
            _csprojService = csprojService;
        }

        protected override async Task OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _options.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _options.ReplaceFile = OptionReplaceFile.Value;

            _csprojService.IsValidProject(_options.DirectoryWorking);

            await _repositoryService.CreateRepositories();
        }
    }
}