using System;
using System.IO;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class StartService : IStartService
    {
        private readonly ILogger _logger;
        private readonly ICsprojService _csproj;
        private readonly IOptionsCommand _optionsCommand;

        public StartService(
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
            _logger.LogTrace("Preparing Solution...");

            CreateFolderEntities();
            CkeckPackageReference();
        }

        private void CreateFolderEntities()
        {
            string pathmap = Path.Combine(_optionsCommand.DirectoryWorking, Folder.Entities);

            if (!Directory.Exists(pathmap))
                Directory.CreateDirectory(pathmap);

            _csproj.FolderInclude(Folder.Entities);
        }

        private void CkeckPackageReference()
        {
            if (!_csproj.ExistPackageReference("Microsoft.EntityFrameworkCore"))
                _logger.LogTrace("You must install the package Microsoft.EntityFrameworkCore");

            if (!_csproj.ExistPackageReference("Microsoft.EntityFrameworkCore.Relational"))
                _logger.LogTrace("You must install the package Microsoft.EntityFrameworkCore.Relational");

            if (!_csproj.ExistPackageReference("Microsoft.EntityFrameworkCore.Design"))
                _logger.LogTrace("You must install the package Microsoft.EntityFrameworkCore.Design");

            if (!_csproj.ExistPackageReference("Microsoft.AspNetCore.Http.Abstractions"))
                _logger.LogTrace("You must install the package Microsoft.AspNetCore.Http.Abstractions");
        }
    }
}
