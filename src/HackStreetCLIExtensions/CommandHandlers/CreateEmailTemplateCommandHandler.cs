using HackStreetCLIExtensions.Models;
using Microsoft.Extensions.Options;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Sdk;
using Stylelabs.M.Sdk.Contracts.Notifications;
using Stylelabs.M.Sdk.Extensions;
using Stylelabs.M.Sdk.Models.Typed;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;
using System.Globalization;

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
            Renderer.WriteLine(Parameters.Name);
            Renderer.WriteLine(Parameters.Label);
            Renderer.WriteLine(Parameters.Subject);
            Renderer.WriteLine(Parameters.Body);
            Renderer.RenderJson(Parameters.VariableName);
            Renderer.RenderJson(Parameters.VariableType);

            var emailTemplateName = Parameters.Name;
            var emailTemplateLabel = Parameters.Label;
            var emailTemplateSubject = Parameters.Subject;
            var emailTemplateBody = Parameters.Body;
            var emailTemplateDescription = Parameters.Description;

            var emailTemplateVariableNames = Parameters.VariableName;
            var emailTemplateVariableTypes = Parameters.VariableType.ToList();

            var client = Client.Value;
            CultureInfo enUs = CultureInfo.GetCultureInfo("en-US");
            var entity = client.EntityFactory.CreateAsync(Constants.MailTemplate.DefinitionName).ConfigureAwait(false).GetAwaiter().GetResult();
            var template = client.TypedEntityFactory.FromEntity<IMailTemplateEntity>(entity);
            template.Name = emailTemplateName;
            template.Subject[enUs] = emailTemplateSubject;
            template.Description[enUs] = emailTemplateDescription;
            template.Body[enUs] = emailTemplateBody;
            template.SetPropertyValue("M.Mailing.TemplateLabel", enUs, emailTemplateLabel);

            if (emailTemplateVariableNames != null) {
                var index = 0;
                List<TemplateVariable> templateVariables = new List<TemplateVariable>();
                foreach (var emailTemplateVariableName in emailTemplateVariableNames)
                {
                    var templateVariable = new TemplateVariable
                    {
                        Name = emailTemplateVariableName,
                        VariableType = emailTemplateVariableTypes[index]
                    };
                    templateVariables.Add(templateVariable);
                    index++;
                }
                template.SetTemplateVariables(templateVariables);
            }

            var entityId = client.Entities.SaveAsync(template).ConfigureAwait(false).GetAwaiter().GetResult();
            var emailEntityLink = client.LinkHelper.EntityToLinkAsync(entityId).ConfigureAwait(false).GetAwaiter().GetResult();
            var hostName = new Uri(emailEntityLink.Uri).Host;
            var emailLinkUrl = $"https://{hostName}/en-us/admin/emailtemplates/detail/{entityId}";
            Renderer.WriteLine("Email Template Created Successfully");
            Renderer.WriteLine(emailLinkUrl);
            return Task.FromResult(0);
        }
    }
}
