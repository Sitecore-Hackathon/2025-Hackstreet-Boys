using Microsoft.Extensions.DependencyInjection;
using Sitecore.CH.Cli.Core.Abstractions.Infrastructure;
using Sitecore.CH.Cli.Core.Extensions;
using HackStreetCLIExtensions.Commands;
using HackStreetCLIExtensions.Models;
using System.CommandLine;
using HackStreetCLIExtensions.CommandHandlers;

namespace HackStreetCLIExtensions.Plugins
{
    public class HackStreetPlugin : IPlugin
    {
        //This is required to register the command handler
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<ExportAssetCommandHandler, ExportAssetParameters>();
            services.AddCommandHandler<CreateEmailTemplateCommandHandler, CreateEmailTemplateCommandParameters>();
        }
        public void RegisterCommands(ICommandRegistry registry)
        {
            registry.RegisterCommandGroup(
     "hackstreet", new List<Command> { new ExportAssetCommand(), new CreateEmailTemplateCommand() }, "Hackstreet plugin for cli extension");
        }
    }
}
