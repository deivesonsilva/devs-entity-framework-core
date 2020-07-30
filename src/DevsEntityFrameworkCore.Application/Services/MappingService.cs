﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ICsprojService _csproj;
        private readonly IOptionsCommand _optionsCommand;

        public MappingService(
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

            _logger.LogTrace("Creating Mappings...");

            foreach (EntityFile entity in entities)
            {
                await CreateMappingFile(entity);
            }

            _csproj.FolderInclude(Folder.Mappings);

            await CreateContextFile(entities);
        }

        private async Task CreateMappingFile(EntityFile entity)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine($"using {entity.Namespace}.{Folder.Entities};");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine();
            sb.AppendLine($"namespace {entity.Namespace}.{Folder.Mappings}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public class {entity.ClassName}Map : IEntityTypeConfiguration<{entity.ClassName}>");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public void Configure(EntityTypeBuilder<{entity.ClassName}> builder)");
            sb.AppendLine($"{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}{identy}builder.ToTable(\"{entity.ClassName}\");");
            sb.AppendLine($"{identy}{identy}{identy}builder.HasKey(x => x.Id);");

            foreach (EntityProperty prop in entity.Properties)
            {
                sb.AppendLine($"{identy}{identy}{identy}builder.Property(x => x.{prop.Name});");
            }

            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            string pathmap = Path.Combine(_optionsCommand.DirectoryWorking, Folder.Mappings);
            string pathfilemap = Path.Combine(pathmap, $"{entity.ClassName}Map.cs");

            if (!Directory.Exists(pathmap))
                Directory.CreateDirectory(pathmap);

            if (File.Exists(pathfilemap) && !_optionsCommand.ReplaceFile)
            {
                _logger.LogTrace($"{entity.ClassName}Map.cs not created. A file with that name already exists");
                return;
            }
                
            using (var stream = new FileStream(pathfilemap, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, useAsync: true))
            {
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Close();
            }
            _logger.LogTrace($"{entity.ClassName}Map.cs created");
        }

        private async Task CreateContextFile(ICollection<EntityFile> entities)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";
            EntityFile entity = entities?.Single();

            sb.AppendLine($"using {entity.Namespace}.{Folder.Entities};");
            sb.AppendLine($"using {entity.Namespace}.{Folder.Mappings};");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine();
            sb.AppendLine($"namespace {entity.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public class RepositoryContext : DbContext");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public RepositoryContext(DbContextOptions<RepositoryContext> options)");
            sb.AppendLine($"{identy}{identy}{identy}: base(options) " + "{ }");
            sb.AppendLine();
            sb.AppendLine($"{identy}{identy}protected override void OnModelCreating(ModelBuilder builder)");
            sb.AppendLine($"{identy}{identy}" + "{");
            foreach(EntityFile enti in entities)
            {
                sb.AppendLine($"{identy}{identy}{identy}builder.ApplyConfiguration(new {enti.ClassName}Map());");
            }
            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine();
            foreach (EntityFile enti in entities)
            {
                sb.AppendLine($"{identy}{identy}public DbSet<{enti.ClassName}> {enti.ClassName}s "+"{ get; set; }");
            }
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            string pathfilemap = Path.Combine(_optionsCommand.DirectoryWorking, $"RepositoryContext.cs");

            if (File.Exists(pathfilemap) && !_optionsCommand.ReplaceFile)
            {
                _logger.LogTrace($"RepositoryContext.cs not created. A file with that name already exists");
                return;
            }

            using (var stream = new FileStream(pathfilemap, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, useAsync: true))
            {
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Close();
            }
            _logger.LogTrace($"RepositoryContext.cs created");
        }
    }
}