using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class ConfigureInjectionService : IConfigureInjectionService
    {
        private readonly ILogger _logger;
        private readonly IOptionsCommand _options;
        private readonly ICsprojService _csproj;
        private readonly IFileService _fileService;
        private readonly IEntityService _entityService;

        public ConfigureInjectionService(
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

        public async Task CreateConfigureInjection()
        {
            _logger.LogTrace("Creating Initialize Infrastructure...");

            string filename = "InitializeInfrastructure.cs";
            string pathname = Path.Combine(_csproj.ProjectPath, filename);

            if (File.Exists(pathname) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            ICollection<EntityMap> entitiesMap = await _entityService.GetEntities();

            if (entitiesMap.Count == 0)
                _logger.LogTrace($"Cannot load entities");

            string content = GenerateInitializeFile(entitiesMap);

            await _fileService.SaveFile(content, pathname);
            _logger.LogTrace($"InitializeInfrastructure.cs created");
        }

        private string GenerateInitializeFile(ICollection<EntityMap> entitiesMap)
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            if (!File.Exists(Path.Combine(_csproj.ProjectPath, "RepositoryContext.cs")))
                _logger.LogTrace("RepositoryContext not found");

            if (!File.Exists(Path.Combine(_csproj.ProjectPath, "RepositoryUnitWork.cs")))
                _logger.LogTrace("RepositoryUnitWork not found");

            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Interfaces};");
            sb.AppendLine($"using {_csproj.ProjectNamespace}.{Folder.Repositories};");
            sb.AppendLine();
            sb.AppendLine($"namespace {_csproj.ProjectNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public static class InitializeInfrastructure");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)");
            sb.AppendLine($"{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}{identy}//You must install Mysql provider if you are using this");
            sb.AppendLine($"{identy}{identy}{identy}services.AddDbContext<RepositoryContext>(options =>");
            sb.AppendLine($"{identy}{identy}{identy}{identy}options.UseMySql(");
            sb.AppendLine($"{identy}{identy}{identy}{identy}{identy}configuration.GetConnectionString(\"DefaultConnection\")));");
            sb.AppendLine();
            sb.AppendLine($"{identy}{identy}{identy}services.AddScoped<IRepositoryUnitWork, RepositoryUnitWork>();");

            foreach (EntityMap map in entitiesMap)
            {
                sb.AppendLine($"{identy}{identy}{identy}services.AddTransient<I{map.ClassName}Repository, {map.ClassName}Repository>();");

                if (!File.Exists(Path.Combine(_csproj.ProjectPath, Folder.Repositories, $"{map.ClassName}Repository.cs")))
                    _logger.LogTrace($"{map.ClassName}Repository.cs not found");
            }

            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine();
            sb.AppendLine($"{identy}{identy}public static void ConfigureUpdateDatebase(this IApplicationBuilder app)");
            sb.AppendLine($"{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}{identy}using (var serviceScope = app.ApplicationServices");
            sb.AppendLine($"{identy}{identy}{identy}{identy}.GetRequiredService<IServiceScopeFactory>()");
            sb.AppendLine($"{identy}{identy}{identy}{identy}.CreateScope())");
            sb.AppendLine($"{identy}{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}{identy}{identy}using (var context = serviceScope.ServiceProvider.GetService<RepositoryContext>())");
            sb.AppendLine($"{identy}{identy}{identy}{identy}" + "{");
            sb.AppendLine($"{identy}{identy}{identy}{identy}{identy}context.Database.EnsureCreated();");
            sb.AppendLine($"{identy}{identy}{identy}{identy}" + "}");
            sb.AppendLine($"{identy}{identy}{identy}" + "}");
            sb.AppendLine($"{identy}{identy}" + "}");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
