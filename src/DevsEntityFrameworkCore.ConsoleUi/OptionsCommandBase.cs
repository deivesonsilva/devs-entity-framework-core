using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DevsEntityFrameworkCore.ConsoleUi
{
    public abstract class OptionsCommandBase : CommandBase
    {
        protected OptionsCommandBase(ILoggerFactory logger, IConsole console)
            : base(logger, console) { }

        [Option("-d <directory>", Description = "The root project directory")]
        public string OptionDirectoryWorking { get; set; }

        [Option("--replace", Description = "Replace file if exists")]
        public bool? OptionReplaceFile { get; set; }
    }
}
