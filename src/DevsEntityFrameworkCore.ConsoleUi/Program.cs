using System;
using System.Reflection;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;
using DevsEntityFrameworkCore.Application.Models;
using DevsEntityFrameworkCore.Application.Services;
using DevsEntityFrameworkCore.ConsoleUi.SubCommands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DevsEntityFrameworkCore.ConsoleUi
{
    [Command("devs", "Fast Development with Entity Framework Core")]
    [Subcommand(typeof(InitializeCommand))]
    [Subcommand(typeof(MappingCommand))]
    [Subcommand(typeof(RepositoryCommand))]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    public class Program : CommandBase
    {
        protected override Task OnExecute(CommandLineApplication application)
        {
            application.ShowHelp();
            return base.OnExecute(application);
        }

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} {Level:u1} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                var services = new ServiceCollection()
                    .AddLogging(logger => logger
                        .AddSerilog()
                        .SetMinimumLevel(LogLevel.Information)
                    )
                    .AddSingleton(PhysicalConsole.Singleton)
                    .AddScoped<ICsprojService, CsprojService>()
                    .AddScoped<IOptionsCommand, OptionsCommand>()
                    .AddTransient<IMappingService, MappingService>()
                    .AddTransient<IRepositoryService, RepositoryService>()
                    .AddTransient<IContextService, ContextService>()
                    .AddTransient<IFileService, FileService>()
                    .AddTransient<IEntityService, EntityService>()
                    .AddTransient<IUnitWorkService, UnitWorkService>()
                    .AddTransient<IConfigureInjectionService, ConfigureInjectionService>()
                    .BuildServiceProvider();

                var app = new CommandLineApplication<Program>();

                app.Conventions
                    .UseDefaultConventions()
                    .UseConstructorInjection(services);

                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static string GetVersion()
        {
            var type = typeof(Program);
            var versionAttribute = type.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return versionAttribute.InformationalVersion;
        }
    }
}
