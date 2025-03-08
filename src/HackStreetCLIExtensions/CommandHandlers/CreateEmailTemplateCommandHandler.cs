using HackStreetCLIExtensions.Models;
using HackStreetCLIExtensions.Views;
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
            Parameters = parameters?.Value!;
        }
        public CreateEmailTemplateCommandParameters Parameters { get; set; }

        /// <summary>
        /// This command creates an Email Template in Sitecore Content Hub based on given Name, Label, Subject, Body, Description and Variables.
        /// </summary>
        /// <param name="context">The command invocation context.</param>
        /// <returns>Sitecore Content Hub specific URL of the Email Template.</returns>
        public override Task<int> InvokeAsync(InvocationContext context)
        {
            try
            {
                Renderer.RenderView(new InfoView("Starting creation of Email Template in Sitecore Content Hub"));
                Renderer.RenderView(new MessageView($"Email Name: {Parameters.Name}"));
                Renderer.RenderView(new MessageView($"Email Label: {Parameters.Label}"));
                Renderer.RenderView(new MessageView($"Email Subject: {Parameters.Subject}"));
                Renderer.RenderView(new MessageView($"Email Body: {Parameters.Body}"));
                Renderer.RenderView(new MessageView($"Email Description: {Parameters.Description}"));            
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

                if (emailTemplateVariableNames != null)
                {
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
                Renderer.RenderView(new SuccessView($"Email Template Url: {emailLinkUrl}"));
                Renderer.RenderView(new SuccessView("Mail Template Generated Successfully!!!"));
                return Task.FromResult(0);
            }
            catch (Exception)
            {
                Renderer.RenderView(new ErrorView("Some error occuring while creating email template"));
                return Task.FromResult(0);
            }
        }
    }
}
