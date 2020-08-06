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
    public class EntityService : IEntityService
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly ICsprojService _csproj;
        private readonly IOptionsCommand _options;

        public EntityService(
            ILoggerFactory logger,
            IFileService fileService,
            ICsprojService csproj,
            IOptionsCommand options)
        {
            _logger = logger.CreateLogger(GetType());
            _fileService = fileService;
            _csproj = csproj;
            _options = options;
        }

        public async Task<ICollection<EntityMap>> GetEntities(bool includeProperty = false)
        {
            List<EntityMap> result = new List<EntityMap>();

            string[] filenamelist = _fileService.GetFileFromFolder(Path.Combine(_csproj.ProjectPath, Folder.Entities));

            foreach (string pathfile in filenamelist)
            {
                EntityMap entity = new EntityMap();
                string fileContent = await _fileService.GetContentFile(pathfile);

                if (!fileContent.Contains("public class"))
                    continue;

                string[] lines = fileContent.Split("public");
                
                foreach (string lin in lines) {

                    if (lin.Contains("using "))
                        continue;

                    if (lin.Contains("class "))
                        entity.ClassName = GetClassName(lin);   
                }

                if (includeProperty)
                    entity.Properties = GetProperties(fileContent);

                result.Add(entity);
            }
            return result;
        }

        private string GetClassName(string content)
        {
            string classname = content;

            if (content.Contains(":"))
                classname = (content.Split(":"))[0];

            classname = classname
                .Replace("class ", string.Empty)
                .Replace(" ", string.Empty);

            return classname;
        }

        private ICollection<EntityPropertyMap> GetProperties(string filecontent)
        {
            ICollection<EntityPropertyMap> properties = new List<EntityPropertyMap>();
            string[] splitPublic = filecontent.Split("public");
            
            foreach (string line in splitPublic)
            {
                //remove line with "class"
                if (line.Contains("class"))
                    continue;

                //remove line with "()" method
                if (line.Contains("() {"))
                    continue;

                //remove line with "using"
                if (line.Contains("using"))
                    continue;

                //remove get; set;
                string[] splitcolch = line.Split("{");

                string[] content = splitcolch[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                EntityPropertyMap prop = new EntityPropertyMap
                {
                    Type = content[0],
                    Name = content[1]
                };
                properties.Add(prop);
            }
            return properties;
        }

        public async Task CreateAbstractEntity()
        {
            _logger.LogTrace("Preparing Entities Folder...");
            string filename = "Entity.cs";
            string pathname = Path.Combine(_csproj.ProjectPath, Folder.Entities, filename);

            if (File.Exists(pathname) && !_options.ReplaceFile)
            {
                _logger.LogTrace($"{filename} not created. A file with that name already exists");
                return;
            }

            string content = GenerateAbstractEntityFile();

            await _fileService.SaveFile(content, pathname);
            _logger.LogTrace($"Entity.cs created");

            _csproj.FolderInclude(Folder.Entities);
        }

        private string GenerateAbstractEntityFile()
        {
            StringBuilder sb = new StringBuilder();
            string identy = "   ";

            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_csproj.ProjectNamespace}.{Folder.Entities}");
            sb.AppendLine("{");
            sb.AppendLine($"{identy}public abstract class Entity");
            sb.AppendLine($"{identy}" + "{");
            sb.AppendLine($"{identy}{identy}[Key]");
            sb.AppendLine($"{identy}{identy}[DatabaseGenerated(DatabaseGeneratedOption.None)]");
            sb.AppendLine($"{identy}{identy}Public Guid Id "+ "{ get; set; } = Guid.NewGuid();");
            sb.AppendLine($"{identy}" + "}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
