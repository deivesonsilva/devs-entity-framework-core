using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using McMaster.Extensions.CommandLineUtils;

namespace DevsEntityFrameworkCore.ConsoleUi.SubCommands
{
    [Command(FullName = "initialize", Name = "init", Description = "Used to configure the solution")]
    public class InitializeCommand : OptionsCommandBase
    {
        [Option("--c", Description = "Used to create the Context file")]
        public bool? OptionContext { get; set; }

        [Option("--u", Description = "Used to create the UnitOfWork file")]
        public bool? OptionUnitOfWork { get; set; }

        [Option("--r", Description = "Used to create the RepositoryBase file")]
        public bool? OptionRepositoryBase { get; set; }

        [Option("--i", Description = "Used to create the Initialize file")]
        public bool? OptionInitialize { get; set; }

        [Option("--all", Description = "Execute all commands")]
        public bool? OptionRunAll { get; set; }

        private readonly IOptionsCommand _options;
        private readonly IContextService _contextService;
        private readonly IRepositoryService _repositoryService;
        private readonly IMappingService _mappingService;
        private readonly IUnitWorkService _unitWorkService;
        private readonly IConfigureInjectionService _configureInjectionService;
        private readonly ICsprojService _csprojService;
        private readonly IEntityService _entityService;

        public InitializeCommand(
            IOptionsCommand options,
            IContextService contextService,
            IRepositoryService repositoryService,
            IMappingService mappingService,
            IUnitWorkService unitWorkService,
            IConfigureInjectionService configureInjectionService,
            ICsprojService csprojService,
            IEntityService entityService)
        {
            _options = options;
            _contextService = contextService;
            _repositoryService = repositoryService;
            _mappingService = mappingService;
            _unitWorkService = unitWorkService;
            _configureInjectionService = configureInjectionService;
            _csprojService = csprojService;
            _entityService = entityService;
        }

        protected override async Task OnExecute(CommandLineApplication application)
        {
            if (!string.IsNullOrEmpty(OptionDirectoryWorking))
                _options.DirectoryWorking = OptionDirectoryWorking;

            if (OptionReplaceFile.HasValue)
                _options.ReplaceFile = OptionReplaceFile.Value;

            if (OptionContext.HasValue)
                _options.CreateContext = OptionContext.Value;

            if (OptionUnitOfWork.HasValue)
                _options.CreateUnitOfWork = OptionUnitOfWork.Value;

            if (OptionRepositoryBase.HasValue)
                _options.CreateRepositoryBase = OptionRepositoryBase.Value;

            if (OptionInitialize.HasValue)
                _options.CreateInitialize = OptionInitialize.Value;

            if (OptionRunAll.HasValue)
                _options.RunAll = OptionRunAll.Value;

            _csprojService.IsValidProject(_options.DirectoryWorking);

            if ((!_options.CreateContext &&
                !_options.CreateUnitOfWork &&
                !_options.CreateRepositoryBase &&
                !_options.CreateInitialize) || _options.RunAll)
            await _entityService.CreateAbstractEntity();

            if (_options.RunAll)
            {
                await _mappingService.CreateMappingFiles();
                await _repositoryService.CreateRepositories();
            }

            if (_options.CreateContext || _options.RunAll)
                await _contextService.CreateContextFile();

            if (_options.CreateUnitOfWork || _options.RunAll)
                await _unitWorkService.CreateUnitWorkFile();

            if (_options.CreateRepositoryBase || _options.RunAll)
                await _repositoryService.CreateRepositoryBase();

            if (_options.CreateInitialize || _options.RunAll)
                await _configureInjectionService.CreateConfigureInjection();

            _csprojService.ExistPackageReference();
        }
    }
}