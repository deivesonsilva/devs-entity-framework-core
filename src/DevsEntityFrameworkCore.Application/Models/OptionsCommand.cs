using System;
using DevsEntityFrameworkCore.Application.Interfaces;

namespace DevsEntityFrameworkCore.Application.Models
{
    public class OptionsCommand : IOptionsCommand
    {
        public string DirectoryWorking { get; set; } = Environment.CurrentDirectory;
        public bool ReplaceFile { get; set; } = false;
    }
}
