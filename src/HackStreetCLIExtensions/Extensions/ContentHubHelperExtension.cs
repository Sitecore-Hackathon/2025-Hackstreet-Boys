using OfficeOpenXml;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.WebClient;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace HackStreetCLIExtensions.Extensions
{
    public static class ContentHubHelperExtension
    {
        public static string FetchAssetPublicLink(IWebMClient client, IEntity entity, IOutputRenderer renderer)
        {
            string publicLink = string.Empty;
            var publicLinks = entity.GetRelation<IParentToManyChildrenRelation>("AssetToPublicLink");
            if (publicLinks != null && publicLinks.GetIds() != null && publicLinks.GetIds().Any())
            {
                Query q = Query.CreateQuery(entities =>
                  from e in entities
                  where e.Id.In(publicLinks.Children)
                   && e.Property("Resource") == "downloadOriginal"
                  select e);

                IEntity publicLinkentity = client.Querying.SingleAsync(q, EntityLoadConfiguration.Default).Result;
                if (publicLinkentity != null)
                {
                    renderer.WriteLine("Public Link exist :");
                    var relativeUrl = publicLinkentity.GetPropertyValue<string>("RelativeUrl");
                    var versionHash = publicLinkentity.GetPropertyValue<string>("VersionHash");
                    return $"/api/public/content/{relativeUrl}?v={versionHash}";
                }
            }
            renderer.WriteLine("Public Link does not exist");
            return publicLink;
        }
        public static void ExportToExcel(List<dynamic> data, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("M.Asset");

                // Add headers
                worksheet.Cells[1, 1].Value = "Title";
                worksheet.Cells[1, 2].Value = "Identifier";
                worksheet.Cells[1, 3].Value = "PublicLink";

                // Add data
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Title;
                    worksheet.Cells[i + 2, 2].Value = data[i].Identifier;
                    worksheet.Cells[i + 2, 3].Value = data[i].PublicLink;
                }

                // Save the file
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }
        }
    }
}
