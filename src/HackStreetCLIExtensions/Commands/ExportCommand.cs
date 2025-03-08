using HackStreetCLIExtensions.CommandHandlers;
using HackStreetCLIExtensions.Extensions;
using HackStreetCLIExtensions.Messages;
using Sitecore.CH.Cli.Core.Abstractions.Commands;

namespace HackStreetCLIExtensions.Commands
{
    public class ExportCommand : BaseCommand<ExportCommandHandler>
    {
        public ExportCommand() : base("export", "Export Assets")
        {
            AddOption<string>(ExportMessages.ExportCommandQuery, false, new string[2]
            {
                "--query",
                "-q"
            });
            AddOption<string>(ExportMessages.ExportCommandLocation, false, new string[2]
            {
                "--location",
                "-l"
            });
            AddOption<string>(ExportMessages.ExportCommandFields, false, new string[2]
            {
                "--fields",
                "-f"
            });
        }
    }
}