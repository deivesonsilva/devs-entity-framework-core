using System;
using System.IO;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class UnitWorkService : IUnitWorkService
    {
        private readonly ILogger _logger;
        private readonly IOptionsCommand _options;
        private readonly ICsprojService _csproj;
        private readonly IFileService _fileService;

        public UnitWorkService(
            ILoggerFactory logger,
            IOptionsCommand options,
            ICsprojService csproj,
            IFileService fileService)
        {
            _logger = logger.CreateLogger(GetType());
            _options = options;
            _csproj = csproj;
            _fileService = fileService;
        }

        public async Task CreateUnitWorkFile()
        {
            _logger.LogTrace("Creating UnitWork...");

            await CreateInterfaceUnitWork();
            await CreateClassUnitWork();
        }

        private async Task CreateInterfaceUnitWork()
        {
            string filename = "IRepositoryUnitWork.cs";
            string pathname = Path.Combine(_csproj.ProjectPath, Folder.Interfaces);
            string fullpath = Path.Combine(pathname, filename);

            if (File.Exists(fullpath) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            string pathcontextfile = Path.Combine(_csproj.ProjectPath, "RepositoryContext.cs");

            if (!File.Exists(pathcontextfile))
                throw new Exception("RepositoryContext.cs not found");

            string content = await _fileService.GetContentFileFromUrl("https://raw.githubusercontent.com/deivesonsilva/devs-entity-framework-core/master/docs/IRepositoryUnitWork.cs");

            if (string.IsNullOrEmpty(content))
                throw new Exception("Cannot load template IUnitOfWork from Github");

            content = content.Replace("//[ns-rep]", $"{_csproj.ProjectNamespace}.{Folder.Interfaces}");

            await _fileService.SaveFile(content, fullpath);
            _logger.LogTrace($"{filename} created");
        }

        private async Task CreateClassUnitWork()
        {
            string filename = "RepositoryUnitWork.cs";
            string fullpath = Path.Combine(_csproj.ProjectPath, filename);
            
            if (File.Exists(fullpath) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            string content = await _fileService.GetContentFileFromUrl("https://raw.githubusercontent.com/deivesonsilva/devs-entity-framework-core/master/docs/RepositoryUnitWork.cs");

            if (string.IsNullOrEmpty(content))
                throw new Exception("Cannot load template UnitOfWork from Github");

            content = content
                .Replace("//[us-rep]", $"using {_csproj.ProjectNamespace}.{Folder.Interfaces};")
                .Replace("//[ns-rep]", _csproj.ProjectNamespace);

            await _fileService.SaveFile(content, fullpath);
            _logger.LogTrace($"{filename} created");
        }
    }
}
