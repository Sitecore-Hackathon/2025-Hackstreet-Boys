using Microsoft.Extensions.Options;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using HackStreetCLIExtensions.Models;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;

namespace HackStreetCLIExtensions.CommandHandlers
{
    public class ImportCommandHandler : BaseCommandHandler
    {
        public ImportCommandHandler(Lazy<IWebMClient> client, IOutputRenderer renderer, IOptions<ImportParameters> parameters) : base(client, renderer)
        {
            Parameters = parameters?.Value;
        }
        public ImportParameters Parameters { get; set; }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Renderer.WriteLine(Parameters.Location);
            var client = Client.Value;
            var location = Parameters.Location;
            var assetDefinition = client.EntityDefinitions.GetAsync("M.Asset").ConfigureAwait(false).GetAwaiter().GetResult();
            if (assetDefinition != null)
            {
            }
            return Task.FromResult(0);
        }
    }
}
