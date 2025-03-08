using HackStreetCLIExtensions.Views;
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
        /// <summary>
        /// Fetch Asset Public Link
        /// </summary>
        /// <param name="client">IWebMClient.</param>
        /// <param name="entity">IEntity.</param>
        /// <param name="renderer">IOutputRenderer.</param>
        /// <returns>string.</returns>
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
                var publicLinkentities = client.Querying.QueryAsync(q, EntityLoadConfiguration.Default).Result;
                if (publicLinkentities != null && publicLinkentities.Items.Count>0)
                {
                    var publicLinkEntity = publicLinkentities.Items.FirstOrDefault();
                    var publicEntityLink = client.LinkHelper.EntityToLinkAsync(publicLinkEntity!.Id!.Value).ConfigureAwait(false).GetAwaiter().GetResult();
                    var relativeUrl = publicLinkEntity.GetPropertyValue<string>("RelativeUrl");
                    var versionHash = publicLinkEntity.GetPropertyValue<string>("VersionHash");
                    var hostName = new Uri(publicEntityLink.Uri).Host;
                    return $"https://{hostName}/api/public/content/{relativeUrl}?v={versionHash}";
                }
            }
            return publicLink;
        }
        /// <summary>
        /// Export Entities to Excel.
        /// </summary>
        /// <param name="data">IEnumerable<JObject>.</param>
        /// <param name="keys">IEnumerable<string>.</param>
        /// <param name="renderer">IOutputRenderer.</param>
        /// <param name="filePath">string.</param>
        public static void ExportToExcel(IEnumerable<JObject> data, IEnumerable<string> keys, IOutputRenderer renderer, string filePath)
        {
            renderer.RenderView(new InfoView($"Total assets being exported to excel are {data.Count()}."));
            renderer.RenderView(new InfoView("Exporting to excel file"));
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var jsonItems = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(JsonConvert.SerializeObject(data));

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("M.Asset");

                worksheet.Cells["A1"].LoadFromDictionaries(jsonItems, true, TableStyles.None, keys);
                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
                renderer.RenderView(new SuccessView("Excel Export Completed!!!"));
            }
        }
    }
}