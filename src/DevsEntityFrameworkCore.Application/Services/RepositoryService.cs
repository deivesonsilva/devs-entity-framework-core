using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly ILogger _logger;
        private readonly ICsprojService _csproj;
        private readonly IOptionsCommand _options;
        private readonly IEntityService _entityService;
        private readonly IFileService _fileService;

        public RepositoryService(
            ILoggerFactory logger,
            ICsprojService csproj,
            IOptionsCommand options,
            IEntityService entityService,
            IFileService fileService)
        {
            _logger = logger.CreateLogger(GetType());
            _csproj = csproj;
            _options = options;
            _entityService = entityService;
            _fileService = fileService;
        }

        public async Task CreateRepositories()
        {
            _logger.LogTrace("Creating Repositories...");

            ICollection<EntityMap> entitiesMap = await _entityService.GetEntities(false);

            if (entitiesMap.Count == 0)
                throw new Exception($"{Path.Combine(_csproj.ProjectPath, Folder.Entities)} not found or is empty");

            foreach (EntityMap map in entitiesMap)
            {
                string content = string.Empty;

                content = GenerateRepositoryInterface(map);
                await SaveRepositoryInterface(content, map);

                content = GenerateRepositoryClass(map);
                await SaveRepositoryClass(content, map);
            }

            _csproj.FolderInclude(Folder.Repositories);
            _csproj.FolderInclude(Folder.Interfaces);
        }

        private string GenerateRepositoryInterface(EntityMap map)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Entities};");
            sb.AppendLine();
            sb.AppendLine($"namespace {_csproj.ProjectNamespace}.{Folder.Interfaces}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public interface I{map.ClassName}Repository : IRepositoryBase<{map.ClassName}>");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private async Task SaveRepositoryInterface(string content, EntityMap map)
        {
            string pathmap = Path.Combine(_csproj.ProjectPath, Folder.Interfaces);
            string pathfile = Path.Combine(pathmap, $"I{map.ClassName}Repository.cs");

            if (File.Exists(pathfile) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"I{map.ClassName}Repository.cs not created. A file with that name already exists");
                return;
            }

            await _fileService.SaveFile(content, pathfile);
            _logger.LogTrace($"I{map.ClassName}Repository.cs created");
        }

        private string GenerateRepositoryClass(EntityMap map)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Entities};");
            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Interfaces};");
            sb.AppendLine();
            sb.AppendLine($"namespace {_csproj.ProjectNamespace}.{Folder.Repositories}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public class {map.ClassName}Repository : RepositoryBase<{map.ClassName}>, I{map.ClassName}Repository");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public {map.ClassName}Repository(RepositoryContext context) : base(context)");
            sb.AppendLine($"{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private async Task SaveRepositoryClass(string content, EntityMap map)
        {
            string pathmap = Path.Combine(_csproj.ProjectPath, Folder.Repositories);
            string pathfile = Path.Combine(pathmap, $"{map.ClassName}Repository.cs");

            if (File.Exists(pathfile) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{map.ClassName}Repository.cs not created. A file with that name already exists");
                return;
            }

            await _fileService.SaveFile(content, pathfile);
            _logger.LogTrace($"{map.ClassName}Repository.cs created");
        }

        public async Task CreateRepositoryBase()
        {
            _logger.LogTrace("Creating RepositoryBase...");

            await GenerateRepositoryBaseInterface();
            await GenerateRepositoryBaseClass();
        }

        private async Task GenerateRepositoryBaseInterface()
        {
            string filename = "IRepositoryBase.cs";
            string pathname = Path.Combine(_csproj.ProjectPath, Folder.Interfaces);
            string fullpath = Path.Combine(pathname, filename);

            if (!Directory.Exists(pathname))
                Directory.CreateDirectory(pathname);

            if (File.Exists(fullpath) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            string content = await _fileService.GetContentFileFromUrl("https://raw.githubusercontent.com/deivesonsilva/devs-entity-framework-core/development/docs/IRepositoryBase.cs");

            if (string.IsNullOrEmpty(content))
                throw new Exception("Cannot load template IRepositoryBase from Github");

            content = content.Replace("//[us-rep]", $"{_csproj.ProjectPath}.{Folder.Entities};");
            content = content.Replace("//[ns-rep]", $"{_csproj.ProjectPath}.{Folder.Interfaces}");

            await _fileService.SaveFile(content, fullpath);
            _logger.LogTrace($"{filename} created");
        }

        private async Task GenerateRepositoryBaseClass()
        {
            string filename = "RepositoryBase.cs";
            string fullpath = Path.Combine(_csproj.ProjectPath, filename);

            if (File.Exists(fullpath) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            string content = await _fileService.GetContentFileFromUrl("https://raw.githubusercontent.com/deivesonsilva/devs-entity-framework-core/development/docs/RepositoryBase.cs");

            if (string.IsNullOrEmpty(content))
                throw new Exception("Cannot load template RepositoryBase from Github");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{_csproj.ProjectPath}.{Folder.Entities};");
            sb.AppendLine($"{_csproj.ProjectPath}.{Folder.Interfaces};");

            content = content.Replace("//[us-rep]", sb.ToString());
            content = content.Replace("//[ns-rep]", $"{_csproj.ProjectPath}");

            await _fileService.SaveFile(content, fullpath);
            _logger.LogTrace($"{filename} created");
        }
    }
}