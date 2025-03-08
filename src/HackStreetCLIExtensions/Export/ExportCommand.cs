using Microsoft.Extensions.Options;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using SitecoreCHCLIExtensions.Models;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;

namespace SitecoreCHCLIExtensions.Export
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
    public class ExportCommandHandler : BaseCommandHandler
    {
        public ExportCommandHandler(Lazy<IWebMClient> client, IOutputRenderer renderer, IOptions<ExportParameters> parameters) : base(client, renderer)
        {
            this.Parameters = parameters?.Value;
        }
        public ExportParameters Parameters { get; set; }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Renderer.WriteLine(this.Parameters.Query);
            Renderer.WriteLine(this.Parameters.Fields);
            Renderer.WriteLine(this.Parameters.Location);
            // Return exit code to indicate success or failure
            return Task.FromResult(0);
        }
    }
}
