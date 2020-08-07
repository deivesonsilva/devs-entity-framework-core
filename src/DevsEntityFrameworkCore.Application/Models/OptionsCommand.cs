using System;
using DevsEntityFrameworkCore.Application.Interfaces;

namespace DevsEntityFrameworkCore.Application.Models
{
    public class OptionsCommand : IOptionsCommand
    {
        public string DirectoryWorking { get; set; } = Environment.CurrentDirectory;
        public bool CreateContext { get; set; } = false;
        public bool CreateUnitOfWork { get; set; } = false;
        public bool CreateRepositoryBase { get; set; } = false;
        public bool CreateInitialize { get; set; } = false;
        public bool RunAll { get; set; } = false;
        public bool ReplaceFile { get; set; } = false;
    }
}
