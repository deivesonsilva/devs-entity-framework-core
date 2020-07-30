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
        private readonly IOptionsCommand _optionsCommand;

        public RepositoryService(
            ILoggerFactory logger,
            ICsprojService csproj,
            IOptionsCommand optionsCommand)
        {
            _logger = logger.CreateLogger(GetType());
            _csproj = csproj;
            _optionsCommand = optionsCommand;
        }

        public async Task Handler()
        {
            ICollection<EntityFile> entities = await _csproj.GetEntitiesFiles();

            if (entities.Count <= 0)
                return;

            _logger.LogTrace("Creating Repositories...");

            foreach (EntityFile entity in entities)
            {
                await CreateRepositoryFile(entity);
            }

            _csproj.FolderInclude(Folder.Repositories);

            _logger.LogTrace("Creating Interfaces...");

            foreach (EntityFile entity in entities)
            {
                await CreateInterfaceFile(entity);
            }

            _csproj.FolderInclude(Folder.Interfaces);
        }

        private async Task CreateRepositoryFile(EntityFile entity)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine($"using {entity.Namespace}.{Folder.Entities};");
            sb.AppendLine($"using {entity.Namespace}.{Folder.Interfaces};");
            sb.AppendLine();
            sb.AppendLine($"namespace {entity.Namespace}.{Folder.Repositories}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public class {entity.ClassName}Repository : RepositoryBase<{entity.ClassName}>, I{entity.ClassName}Repository");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public {entity.ClassName}Repository(RepositoryContext context) : base(context)");
            sb.AppendLine($"{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            string pathmap = Path.Combine(_optionsCommand.DirectoryWorking, Folder.Repositories);
            string pathfilemap = Path.Combine(pathmap, $"{entity.ClassName}Repository.cs");

            if (!Directory.Exists(pathmap))
                Directory.CreateDirectory(pathmap);

            if (File.Exists(pathfilemap) && !_optionsCommand.ReplaceFile)
            {
                _logger.LogTrace($"{entity.ClassName}Repository.cs not created. A file with that name already exists");
                return;
            }

            using (var stream = new FileStream(pathfilemap, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, useAsync: true))
            {
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Close();
            }
            _logger.LogTrace($"{entity.ClassName}Repository.cs created");
        }

        private async Task CreateInterfaceFile(EntityFile entity)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine($"using {entity.Namespace}.{Folder.Entities};");
            sb.AppendLine();
            sb.AppendLine($"namespace {entity.Namespace}.{Folder.Interfaces}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public interface I{entity.ClassName}Repository : IRepositoryBase<{entity.ClassName}>");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            string pathmap = Path.Combine(_optionsCommand.DirectoryWorking, Folder.Interfaces);
            string pathfilemap = Path.Combine(pathmap, $"I{entity.ClassName}Repository.cs");

            if (!Directory.Exists(pathmap))
                Directory.CreateDirectory(pathmap);

            if (File.Exists(pathfilemap) && !_optionsCommand.ReplaceFile)
            {
                _logger.LogTrace($"I{entity.ClassName}Repository.cs not created. A file with that name already exists");
                return;
            }

            using (var stream = new FileStream(pathfilemap, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, useAsync: true))
            {
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Close();
            }
            _logger.LogTrace($"I{entity.ClassName}Repository.cs created");
        }
    }
}