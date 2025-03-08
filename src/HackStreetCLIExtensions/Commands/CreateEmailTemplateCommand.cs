using HackStreetCLIExtensions.CommandHandlers;
using HackStreetCLIExtensions.Messages;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Stylelabs.M.Sdk.Contracts.Notifications;

namespace HackStreetCLIExtensions.Commands
{
    public class CreateEmailTemplateCommand : BaseCommand<CreateEmailTemplateCommandHandler>
    {
        public CreateEmailTemplateCommand() : base("createemailtemplate", "Create Email Template")
        {
            AddOption<string>(CreateEmailTemplateMessages.Name, false, new string[2]
            {
                "--name",
                "-n"
            });
            AddOption<string>(CreateEmailTemplateMessages.Label, false, new string[2]
            {
                "--label",
                "-l"
            });
            AddOption<string>(CreateEmailTemplateMessages.EmailSubject, false, new string[2]
            {
                "--subject",
                "-s"
            });
            AddOption<string>(CreateEmailTemplateMessages.EmailBody, false, new string[2]
            {
                "--body",
                "-b"
            });
            AddOption<string>(CreateEmailTemplateMessages.EmailDescription, false, new string[2]
            {
                "--description",
                "-d"
            });
            AddOption<ICollection<string>>(CreateEmailTemplateMessages.VariablesName, false, new string[2]
            {
                "--variablename",
                "-vn"
            });
            AddEnumArrayOption<TemplateVariableType>(CreateEmailTemplateMessages.VariablesType, false, new string[2]
            {
                "--variabletype",
                "-vt"
            });
        }
    }
}