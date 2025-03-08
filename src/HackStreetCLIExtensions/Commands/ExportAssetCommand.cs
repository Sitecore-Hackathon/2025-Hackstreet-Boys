using HackStreetCLIExtensions.CommandHandlers;
using HackStreetCLIExtensions.Extensions;
using HackStreetCLIExtensions.Messages;
using Sitecore.CH.Cli.Core.Abstractions.Commands;

namespace HackStreetCLIExtensions.Commands
{
    public class ExportAssetCommand : BaseCommand<ExportAssetCommandHandler>
    {
        public ExportAssetCommand() : base("exportasset", "Export Assets")
        {
            AddOption<string>(ExportAssetMessages.ExportCommandQuery, false, new string[2]
            {
                "--query",
                "-q"
            });
            AddOption<string>(ExportAssetMessages.ExportCommandLocation, false, new string[2]
            {
                "--location",
                "-l"
            });
            AddOption<string>(ExportAssetMessages.ExportCommandFields, false, new string[2]
            {
                "--fields",
                "-f"
            });
        }
    }
}