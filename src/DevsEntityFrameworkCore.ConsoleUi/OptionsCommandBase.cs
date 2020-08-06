using McMaster.Extensions.CommandLineUtils;

namespace DevsEntityFrameworkCore.ConsoleUi
{
    public abstract class OptionsCommandBase : CommandBase
    {
        [Option("-d <directory>", Description = "The root project directory")]
        public string OptionDirectoryWorking { get; set; }

        [Option("--replace", Description = "Replace file if exists")]
        public bool? OptionReplaceFile { get; set; } 
    }
}
