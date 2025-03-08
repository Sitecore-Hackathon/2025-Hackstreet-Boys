using HackStreetCLIExtensions.Models;
using Microsoft.Extensions.Options;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;

namespace HackStreetCLIExtensions.CommandHandlers
{
    public class CreateEmailTemplateCommandHandler : BaseCommandHandler
    {
        public CreateEmailTemplateCommandHandler(Lazy<IWebMClient> client, IOutputRenderer renderer, IOptions<CreateEmailTemplateCommandParameters> parameters) : base(client, renderer)
        {
            Parameters = parameters?.Value;
        }
        public CreateEmailTemplateCommandParameters Parameters { get; set; }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Renderer.WriteLine(Parameters.Name);
            Renderer.WriteLine(Parameters.Label);
            Renderer.WriteLine(Parameters.Subject);
            Renderer.WriteLine(Parameters.Body);
            Renderer.RenderJson(Parameters.Body);
            //var name = Parameters.Name;
            //var label = Parameters.Label;
            //var subject = Parameters.Subject;
            //var body = Parameters.Body;
            //var variables = Parameters.Variables;


            return Task.FromResult(0);
        }
    }
}
