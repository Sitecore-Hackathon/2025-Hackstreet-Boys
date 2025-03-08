namespace HackStreetCLIExtensions.Models
{
    /// <summary>
    /// Parameters required for Export
    /// </summary>
    public class ExportParameters
    {
        /// <summary>
        /// Assets of Content Hub will be found/filtered based on this Query
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// The location where the Excel file needs to be exported with Excel File Link. e.g. C:\ContentHubCLI\Assets.xlsx
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// The fields that need to be exported which should be PIPE separated
        /// </summary>
        public string Fields { get; set; }
    }
}
