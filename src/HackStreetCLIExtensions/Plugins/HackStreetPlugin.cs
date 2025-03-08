using Microsoft.Extensions.DependencyInjection;
using Sitecore.CH.Cli.Core.Abstractions.Infrastructure;
using Sitecore.CH.Cli.Core.Extensions;
using SitecoreCHCLIExtensions.Export;
using SitecoreCHCLIExtensions.Models;
using System.CommandLine;

namespace SitecoreCHCLIExtensions.Plugins
{
    public class HackStreetPlugin : IPlugin
    {
        //This is required to register the command handler
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<ExportCommandHandler, ExportParameters>();
        }
        public void RegisterCommands(ICommandRegistry registry)
        {
            registry.RegisterCommandGroup(
     "hackstreet", new List<Command> { new ExportCommand() }, "Hackstreet plugin for cli extension");
        }
    }
}
