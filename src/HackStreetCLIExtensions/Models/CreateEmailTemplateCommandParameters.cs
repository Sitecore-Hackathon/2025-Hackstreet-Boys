using Stylelabs.M.Sdk.Contracts.Notifications;

namespace HackStreetCLIExtensions.Models
{
    public class CreateEmailTemplateCommandParameters
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ICollection<EmailTemplateVariablesModel> Variables { get; set; }
    }
}
