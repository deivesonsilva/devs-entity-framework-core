using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class ContextService : IContextService
    {
        private readonly ILogger _logger;
        private readonly IOptionsCommand _options;
        private readonly ICsprojService _csproj;
        private readonly IFileService _fileService;
        private readonly IEntityService _entityService;

        public ContextService(
            ILoggerFactory logger,
            IOptionsCommand options,
            ICsprojService csproj,
            IFileService fileService,
            IEntityService entityService)
        {
            _logger = logger.CreateLogger(GetType());
            _options = options;
            _csproj = csproj;
            _fileService = fileService;
            _entityService = entityService;
        }

        public async Task CreateContextFile()
        {
            _logger.LogTrace("Creating Repository Context...");

            string filename = "RepositoryContext.cs";
            string pathname = Path.Combine(_csproj.ProjectPath, filename);

            if (File.Exists(pathname) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            ICollection<EntityMap> entitiesMap = await _entityService.GetEntities();
            
            if (entitiesMap.Count == 0)
                _logger.LogTrace($"Cannot load entities");

            string content = GenerateContextFile(entitiesMap);

            await _fileService.SaveFile(content, pathname);
            _logger.LogTrace($"RepositoryContext.cs created");
        }

        private string GenerateContextFile(ICollection<EntityMap> entitiesMap)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Entities};");
            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Mappings};");
            sb.AppendLine();
            sb.AppendLine($"namespace {_csproj.ProjectNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public class RepositoryContext : DbContext");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public RepositoryContext(DbContextOptions<RepositoryContext> options)");
            sb.AppendLine($"{identy}{identy}{identy}: base(options) " + "{ }");
            sb.AppendLine();
            sb.AppendLine($"{identy}{identy}protected override void OnModelCreating(ModelBuilder builder)");
            sb.AppendLine($"{identy}{identy}" + "{");

            foreach (EntityMap map in entitiesMap)
            {
                sb.AppendLine($"{identy}{identy}{identy}builder.ApplyConfiguration(new {map.ClassName}Map());");

                if (!File.Exists(Path.Combine(_csproj.ProjectPath, Folder.Mappings, $"{map.ClassName}Map.cs")))
                    _logger.LogTrace($"{map.ClassName}Map.cs not found");
            }

            if (entitiesMap.Count == 0)
                sb.AppendLine($"{identy}{identy}{identy}//builder.ApplyConfiguration(new EntityNameMap());");

            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine();

            foreach (EntityMap map in entitiesMap)
                sb.AppendLine($"{identy}{identy}public DbSet<{map.ClassName}> {map.ClassName}s " + "{ get; set; }");
            
            if (entitiesMap.Count == 0)
                sb.AppendLine($"{identy}{identy}//public DbSet<EntityName> EntityNames " + "{ get; set; }");

            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");
            return sb.ToString();
        }  
    }
}