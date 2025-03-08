using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Linq;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.WebClient;
using System.Dynamic;
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
                    var publicEntityLink = client.LinkHelper.EntityToLinkAsync(publicLinkentity.Id.Value).ConfigureAwait(false).GetAwaiter().GetResult();
                    renderer.WriteLine("Public Link exist:");
                    var relativeUrl = publicLinkentity.GetPropertyValue<string>("RelativeUrl");
                    var versionHash = publicLinkentity.GetPropertyValue<string>("VersionHash");
                    var hostName = new Uri(publicEntityLink.Uri).Host;
                    return $"https://{hostName}/api/public/content/{relativeUrl}?v={versionHash}";
                }
            }
            renderer.WriteLine("Public Link does not exist");
            return publicLink;
        }
        public static void ExportToExcel(IEnumerable<JObject> data, IEnumerable<string> keys, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var jsonItems = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(JsonConvert.SerializeObject(data));

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("M.Asset");

                worksheet.Cells["A1"].LoadFromDictionaries(jsonItems, true, TableStyles.None, keys);
                // Save the file
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }
        }
    }
}