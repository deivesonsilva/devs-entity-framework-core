using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class CsprojService : ICsprojService
    {
        private readonly ILogger _logger;
        private readonly IOptionsCommand _optionsCommand;

        public CsprojService(
            ILoggerFactory logger,
            IOptionsCommand optionsCommand) {
            _logger = logger.CreateLogger(GetType());
            _optionsCommand = optionsCommand;
        }

        public void FolderInclude(string foldername)
        {
            string filenamecsproj = GetFileNameCsproj();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(filenamecsproj);
            
            XmlNodeList listFolder = xdoc.SelectNodes("Project/ItemGroup/Folder");
            bool folderExist = false;

            foreach (XmlNode n in listFolder)
            {
                if (n.Attributes["Include"].InnerText.Equals(foldername + @"\"))
                    folderExist = true;
            }

            if (folderExist)
                return;

            if (listFolder.Count == 0)
            {
                XmlNode ndProj = xdoc.SelectSingleNode("Project");
                XmlElement xEleItem = xdoc.CreateElement("ItemGroup");
                XmlElement xEleFol = xdoc.CreateElement("Folder");
                xEleFol.SetAttribute("Include", foldername + @"\");
                xEleItem.AppendChild(xEleFol);
                ndProj.AppendChild(xEleItem);
            }
            else
            {
                XmlNode ndItem = xdoc.SelectSingleNode("Project/ItemGroup/Folder").ParentNode;
                XmlElement xEleFol = xdoc.CreateElement("Folder");
                xEleFol.SetAttribute("Include", foldername + @"\");
                ndItem.AppendChild(xEleFol);
            }
            xdoc.Save(filenamecsproj);
            _logger.LogTrace($"{foldername} folder included in .csproj");
        }

        public async Task<ICollection<EntityFile>> GetEntitiesFiles()
        {
            List<EntityFile> entities = new List<EntityFile>();
            string pathentities = Path.Combine(_optionsCommand.DirectoryWorking, Folder.Entities);

            string[] filenamelist = Directory.GetFiles(pathentities, "*.cs", SearchOption.AllDirectories);

            if (filenamelist.Length <= 0)
                throw new Exception($"{pathentities} folder is empty");

            foreach (string pathfile in filenamelist)
            {
                FileInfo file = new FileInfo(pathfile);
                string fileContent = await GetContentFile(pathfile);

                if (string.IsNullOrEmpty(fileContent))
                {
                    _logger.LogTrace($"The file {file.Name} is empty");
                    continue;
                }

                string classname = GetClassName(fileContent);
                
                if (string.IsNullOrEmpty(classname))
                {
                    _logger.LogTrace($"Class Name not found in {file.Name}");
                    continue;
                }

                if (!IsValidClass(fileContent, classname))
                {
                    _logger.LogTrace($"The Class {classname} is not public Class");
                    continue;
                }

                string namespacedesc = GetNameSpace(fileContent);

                if (string.IsNullOrEmpty(namespacedesc))
                {
                    _logger.LogTrace($"Namespace not found in {file.Name}");
                    continue;
                }

                if (!IsValidNameSpace(fileContent, namespacedesc))
                {
                    _logger.LogTrace($"The Namespace {namespacedesc} in {file.Name} is invalid");
                    continue;
                }

                EntityFile entity = new EntityFile();
                entity.ClassName = classname;
                entity.Namespace = namespacedesc;
                entity.Properties = GetProperties(fileContent);
                entities.Add(entity);
                _logger.LogTrace($"{file.Name}");
            }
            return entities;
        }

        private async Task<string> GetContentFile(string path)
        {
            const string reduceMultiSpace = @"[ ]{2,}";

            string content = string.Join(" ", await File.ReadAllLinesAsync(path));

            return Regex.Replace(content.Replace("\t", " "), reduceMultiSpace, " ");
        }

        private string GetClassName(string filecontent)
        {
            string[] splitclassname;

            splitclassname = filecontent.Split("class");

            if (splitclassname == null || splitclassname.Length != 2)
                return string.Empty;

            splitclassname = splitclassname[1].Split("{");
            splitclassname = splitclassname[0].Split(":");

            return splitclassname[0].Replace(" ", string.Empty);
        }

        private string GetNameSpace(string filecontent)
        {
            string[] splitnamespace;

            splitnamespace = filecontent.Split("namespace");

            if (splitnamespace == null || splitnamespace.Length != 2)
                return string.Empty;

            splitnamespace = splitnamespace[1].Split("{");

            return splitnamespace[0]
                .Replace(" ", string.Empty)
                .Replace($".{Folder.Entities}", string.Empty);
        }

        private ICollection<EntityProperty> GetProperties(string filecontent)
        {
            string[] splitPublic = filecontent.Split("public");
            ICollection<EntityProperty> properties = new List<EntityProperty>();

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

                //_logger.LogTrace(splitcolch[0]);
                string[] content = splitcolch[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                EntityProperty prop = new EntityProperty
                {
                    Type = content[0],
                    Name = content[1]
                };
                properties.Add(prop);
            }
            return properties;
        }

        private bool IsValidClass(string filecontent, string classname)
        {
            if (filecontent.Contains($"public class {classname}"))
                return true;

            return false;
        }

        private bool IsValidNameSpace(string filecontent, string namespacedesc)
        {
            if (filecontent.Contains($"namespace {namespacedesc}"))
                return true;

            return false;
        }

        private string GetFileNameCsproj()
        {
            string[] filenamelist = Directory.GetFiles(_optionsCommand.DirectoryWorking, "*.csproj");

            if (filenamelist == null || filenamelist.Length == 0)
                throw new Exception(".csproj file not found");

            if (filenamelist.Length > 1)
                throw new Exception("more than one .csproj file found");

            return filenamelist[0];
        }
    }
}