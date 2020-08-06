using System;
using System.IO;
using System.Xml;
using DevsEntityFrameworkCore.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class CsprojService : ICsprojService
    {
        private string _projectFileName;
        private string _projectNamespace;
        private string _projectPath;
        private readonly ILogger _logger;
        private readonly IFileService _fileService;

        public string ProjectFileName { get { return _projectFileName; } }
        public string ProjectNamespace { get { return _projectNamespace; } }
        public string ProjectPath { get { return _projectPath; } }

        public CsprojService(
            ILoggerFactory logger,
            IFileService fileService) {
            _logger = logger.CreateLogger(GetType());
            _fileService = fileService;
        }

        public void IsValidProject(string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath))
                throw new Exception("Project path is required");

            if (!fullpath.Contains(".csproj") && !fullpath.EndsWith("/"))
                fullpath += "/";

            _projectPath = Path.GetDirectoryName(fullpath);
            _projectFileName = GetProjectName(fullpath);
            _projectNamespace = _projectFileName.Replace(".csproj", string.Empty);  
        }

        public void FolderInclude(string foldername)
        {
            string fullpath = Path.Combine(_projectPath, _projectFileName);
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fullpath);
            
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
            xdoc.Save(fullpath);
            _logger.LogTrace($"{foldername} folder included in .csproj");
        }

        public void ExistPackageReference()
        {
            string[] packages = new string[]
            {
                "Microsoft.EntityFrameworkCore",
                "Microsoft.EntityFrameworkCore.Relational",
                "Microsoft.EntityFrameworkCore.Design",
                "Microsoft.AspNetCore.Http.Abstractions"
            };

            string fullpath = Path.Combine(_projectPath, _projectFileName);

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fullpath);

            XmlNodeList listFolder = xdoc.SelectNodes("Project/ItemGroup/PackageReference");
            bool packageExist = false;

            foreach (string pack in packages)
            {
                foreach (XmlNode n in listFolder)
                {
                    if (n.Attributes["Include"].InnerText.Equals(pack))
                        packageExist = true;
                }

                if (!packageExist)
                    _logger.LogTrace($"You must install the package {pack}");
            }
        }

        private string GetProjectName(string fullpath)
        {
            string result = string.Empty;

            if (fullpath.Contains(".csproj"))
                result = Path.GetFileName(fullpath);

            if (string.IsNullOrEmpty(result))
            {
                string[] filenamelist = _fileService.GetFileFromFolder(_projectPath, "*.csproj");

                if (filenamelist != null && filenamelist.Length == 1)
                    result = Path.GetFileName(filenamelist[0]);
            }

            if (string.IsNullOrEmpty(result))
                throw new Exception(".csproj file not found");

            if (!File.Exists(Path.Combine(Path.GetDirectoryName(fullpath), result)))
                throw new Exception($"{result} file not found");

            return result;
        }  
    }
}