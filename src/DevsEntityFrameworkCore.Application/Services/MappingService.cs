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
    public class MappingService : IMappingService
    {
        private readonly ILogger _logger;
        private readonly IOptionsCommand _options;
        private readonly ICsprojService _csproj;
        private readonly IEntityService _entityService;
        private readonly IFileService _fileService;

        public MappingService(
            ILoggerFactory logger,
            IOptionsCommand options,
            ICsprojService csproj,
            IEntityService entityService,
            IFileService fileService)
        {
            _logger = logger.CreateLogger(GetType());
            _options = options;
            _csproj = csproj;
            _entityService = entityService;
            _fileService = fileService;
        }

        public async Task CreateMappingFiles()
        {
            _logger.LogTrace("Creating Mappings...");

            ICollection<EntityMap> entitiesMap = await _entityService.GetEntities(true);

            if (entitiesMap.Count == 0)
                throw new Exception($"{Path.Combine(_csproj.ProjectPath, Folder.Entities)} not found or is empty");
                
            foreach (EntityMap map in entitiesMap)
            {
                string content = GenerateMappingFile(map);
                await SaveFile(content, map);
            }

            _csproj.FolderInclude(Folder.Mappings);
        }

        private string GenerateMappingFile(EntityMap entity)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Entities};");
            sb.AppendLine();
            sb.AppendLine($"namespace {_csproj.ProjectNamespace}.{Folder.Mappings}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public class {entity.ClassName}Map : IEntityTypeConfiguration<{entity.ClassName}>");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public void Configure(EntityTypeBuilder<{entity.ClassName}> builder)");
            sb.AppendLine($"{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}{identy}builder.ToTable(\"{entity.ClassName}\");");
            sb.AppendLine($"{identy}{identy}{identy}builder.HasKey(x => x.Id);");

            foreach (EntityPropertyMap prop in entity.Properties)
            {
                sb.AppendLine($"{identy}{identy}{identy}builder.Property(x => x.{prop.Name});");
            }

            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private async Task SaveFile(string content, EntityMap entity)
        {
            string pathRoot = Path.Combine(_csproj.ProjectPath, Folder.Mappings);
            string pathfilemap = Path.Combine(pathRoot, $"{entity.ClassName}Map.cs");

            if (!Directory.Exists(pathRoot))
                Directory.CreateDirectory(pathRoot);

            if (File.Exists(pathfilemap) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{entity.ClassName}Map.cs not created. A file with the same name already exists");
                return;
            }

            await _fileService.SaveFile(content, pathfilemap);
            _logger.LogTrace($"{entity.ClassName}Map.cs created");
        }
    }
}