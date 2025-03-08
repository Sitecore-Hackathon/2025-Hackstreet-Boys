using HackStreetCLIExtensions.CommandHandlers;
using HackStreetCLIExtensions.Messages;
using Sitecore.CH.Cli.Core.Abstractions.Commands;

namespace HackStreetCLIExtensions.Commands
{
    public class ImportCommand : BaseCommand<ImportCommandHandler>
    {
        public ImportCommand() : base("import", "Import Assets")
        {
            AddOption<string>(ImportMessages.ImportCommandLocation, false, new string[2]
            {
                "--location",
                "-l"
            });
        }
    }
}
