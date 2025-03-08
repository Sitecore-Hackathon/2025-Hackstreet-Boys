using Stylelabs.M.Sdk.Contracts.Notifications;

namespace HackStreetCLIExtensions.Models
{
    // <summary>
    /// Parameters for Email Template Creation
    /// </summary>
    public class CreateEmailTemplateCommandParameters
    {
        /// <summary>
        /// Name of the Email Template to be created
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Label of the Email Template to be created
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Email Subject for the Email Template 
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Email Body for the Email Template 
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Description of the Email Template visible in the Email Templates section
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Variable Names - as a collection, multiple Variables are supported. The number of Variable Name and Variable Types in a given command should be same.
        /// </summary>
        public ICollection<string> VariableName { get; set; }
        /// <summary>
        /// Variable Types - The Datatype of the Variables - as a collection, multiple Variable Types are supported. The number of Variable Name and Variable Types in a given command should be same.
        /// </summary>
        public ICollection<TemplateVariableType> VariableType { get; set; }
    }
}
